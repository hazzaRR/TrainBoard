using System.Text;
using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using TrainBoard.Entities;

namespace TrainBoard.Services;
public class NetworkConnectivityService : INetworkConnectivityService
{

    private readonly ILogger<NetworkConnectivityService> _logger;
    public bool IsOnline { get; set; }
    public ObjectPath HotspotPath { get; set; }
    public Dictionary<string, ObjectPath> SavedConnections { get; set; }
    public Dictionary<string, AvailableNetwork> AvailableNetworks { get; set; } = [];

    private NetworkConnectivityService(ILogger<NetworkConnectivityService> logger)
    {
        _logger = logger;
    }
    private string GetSignalBars(byte strength)
    {
        if (strength > 85) return "▂▄▆█";
        if (strength > 70) return "▂▄▆_";
        if (strength > 55) return "▂▄__";
        if (strength > 40) return "▂___";
        return "____";
    }

    private string GetSecurityString(NM80211ApSecurityFlags flags)
    {
        var security = new List<string>();

        if (flags.HasFlag(NM80211ApSecurityFlags.NM_802_11_AP_SEC_PAIR_CCMP) &&
            flags.HasFlag(NM80211ApSecurityFlags.NM_802_11_AP_SEC_KEY_MGMT_PSK))
        {
            security.Add("WPA2");
        }
        else if (flags.HasFlag(NM80211ApSecurityFlags.NM_802_11_AP_SEC_PAIR_TKIP) &&
            flags.HasFlag(NM80211ApSecurityFlags.NM_802_11_AP_SEC_KEY_MGMT_PSK))
        {
            security.Add("WPA");
        }

        if (flags.HasFlag(NM80211ApSecurityFlags.NM_802_11_AP_SEC_KEY_MGMT_802_1X))
        {
            security.Add("802.1X");
        }

        if (flags.HasFlag(NM80211ApSecurityFlags.NM_802_11_AP_SEC_PAIR_WEP40) ||
            flags.HasFlag(NM80211ApSecurityFlags.NM_802_11_AP_SEC_PAIR_WEP104))
        {
            security.Add("WEP");
        }

        if (security.Count == 0)
            return "--";

        return string.Join(" ", security);
    }

    public async Task IsInternetConnected(int retries, TimeSpan delay)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "http://www.google.com"));

                    if (response.IsSuccessStatusCode)
                    {
                        IsOnline = true;
                    }
                }
                catch (Exception)
                {
                }
                await Task.Delay(delay);
            }
        }
    }

    public async Task AddNewConnection(Tmds.DBus.Protocol.Connection connection, string ssid, string password, ObjectPath apPath)
    {
        NetworkManagerService nmService = new NetworkManagerService(connection, "org.freedesktop.NetworkManager");
        NetworkManager.DBus.NetworkManager networkManager = nmService.CreateNetworkManager("/org/freedesktop/NetworkManager");
        ObjectPath wirelessDevicePath = await networkManager.GetDeviceByIpIfaceAsync("wlan0");
        Settings settingsService = nmService.CreateSettings("/org/freedesktop/NetworkManager/Settings");
        var wifiSettings = new Dictionary<string, Dictionary<string, VariantValue>>
        {
            ["connection"] = new Dictionary<string, VariantValue>
            {
                ["id"] = $"wifi-{ssid}",
                ["type"] = "802-11-wireless",
                ["interface-name"] = "wlan0",
                ["autoconnect"] = true
            },
            ["802-11-wireless"] = new Dictionary<string, VariantValue>
            {
                ["ssid"] = VariantValue.Array(Encoding.UTF8.GetBytes(ssid)),
                ["mode"] = "infrastructure"
            },
            ["802-11-wireless-security"] = new Dictionary<string, VariantValue>
            {
                ["key-mgmt"] = "wpa-psk",
                ["psk"] = password
            },
            ["ipv4"] = new Dictionary<string, VariantValue>
            {
                ["method"] = "auto"
            },
            ["ipv6"] = new Dictionary<string, VariantValue>
            {
                ["method"] = "ignore"
            }
        };

        _logger.LogInformation($"Attempting to connect to new network '{ssid}'...");

        ObjectPath wifiConnPath = await settingsService.AddConnectionAsync(wifiSettings);
        var activeConn = await networkManager.ActivateConnectionAsync(
            wifiConnPath,
            wirelessDevicePath,
            apPath
        );

        _logger.LogInformation($"Connection activated: {activeConn}");
    }

    public async Task GetSavedConnections(Tmds.DBus.Protocol.Connection connection)
    {

        NetworkManagerService nmService = new NetworkManagerService(connection, "org.freedesktop.NetworkManager");
        Settings settingsService = nmService.CreateSettings("/org/freedesktop/NetworkManager/Settings");

        var savedConnections = new Dictionary<string, ObjectPath>();
        ObjectPath[] allSavedConnections = await settingsService.ListConnectionsAsync();

        foreach (var connPath in allSavedConnections)
        {
            var conn = nmService.CreateConnection(connPath);
            var settings = await conn.GetSettingsAsync();

            if (settings.TryGetValue("connection", out var connSection) &&
                connSection.TryGetValue("type", out var typeValue) &&
                typeValue.GetString() == "802-11-wireless" &&
                settings.TryGetValue("802-11-wireless", out var wifiSection) &&
                wifiSection.TryGetValue("ssid", out var ssidValue))
            {
                //fetch from appSettings for ssid
                string ssid = Encoding.UTF8.GetString(ssidValue.GetArray<byte>());
                if (ssid == "BRboard")
                {
                    HotspotPath = connPath;
                    break;
                }
                else if (!savedConnections.ContainsKey(ssid))
                {
                    savedConnections.Add(ssid, connPath);
                }
            }
        }
        SavedConnections = savedConnections;
    }

    public async Task GetAvailableNetworks(Tmds.DBus.Protocol.Connection connection)
    {

        NetworkManagerService nmService = new NetworkManagerService(connection, "org.freedesktop.NetworkManager");
        NetworkManager.DBus.NetworkManager networkManager = nmService.CreateNetworkManager("/org/freedesktop/NetworkManager");
        ObjectPath wlan0Path = await networkManager.GetDeviceByIpIfaceAsync("wlan0");
        Wireless wirelessDevice = nmService.CreateWireless(wlan0Path);

        _logger.LogInformation("Scanning for available Wi-Fi networks...");
        await wirelessDevice.RequestScanAsync(new Dictionary<string, VariantValue>());
        // await Task.Delay(5000); //Is this even need anymore??
        _logger.LogInformation("Scan complete. Retrieving access points...");

        ObjectPath[] availableAccessPointsPaths = await wirelessDevice.GetAccessPointsAsync();
        ObjectPath activeApPath = await wirelessDevice.GetActiveAccessPointAsync();

        var combinedNetworks = new Dictionary<string, AvailableNetwork>();

        foreach (var apPath in availableAccessPointsPaths)
        {
            var accessPoint = nmService.CreateAccessPoint(apPath);

            byte[] ssidBytes = await accessPoint.GetSsidAsync();
            string ssid = Encoding.UTF8.GetString(ssidBytes);
            string bssid = await accessPoint.GetHwAddressAsync(); 
                                                            
            string uniqueKey = $"{ssid}-{bssid}";
            byte strength = await accessPoint.GetStrengthAsync();

            AvailableNetwork availableNetwork = new()
            {
                Ssid = Encoding.UTF8.GetString(ssidBytes),
                Mode = "Infra",
                Rate = await accessPoint.GetMaxBitrateAsync(),
                Signal = strength,
                Bars = GetSignalBars(strength),
                Security = GetSecurityString((NM80211ApSecurityFlags)await accessPoint.GetRsnFlagsAsync() | (NM80211ApSecurityFlags)await accessPoint.GetWpaFlagsAsync() | (NM80211ApSecurityFlags)await accessPoint.GetFlagsAsync()),
                IsSaved = SavedConnections.ContainsKey(ssid),
                IsActive = apPath == activeApPath,
                ApPath = apPath
            };

            combinedNetworks.TryAdd(uniqueKey, availableNetwork);

        }

        AvailableNetworks = combinedNetworks;
    }
    public async Task JoinSavedNetwork(Tmds.DBus.Protocol.Connection connection, ObjectPath savedConnPath)
    {

        NetworkManagerService nmService = new NetworkManagerService(connection, "org.freedesktop.NetworkManager");
        NetworkManager.DBus.NetworkManager networkManager = nmService.CreateNetworkManager("/org/freedesktop/NetworkManager");
        ObjectPath wlan0Path = await networkManager.GetDeviceByIpIfaceAsync("wlan0");

        var activeConn = await networkManager.ActivateConnectionAsync(
            savedConnPath,
            wlan0Path,
            savedConnPath
        );
        _logger.LogInformation($"Connection activated: {activeConn}");
    }

    public async Task EnableHotspot(Tmds.DBus.Protocol.Connection connection)
    {

        NetworkManagerService nmService = new NetworkManagerService(connection, "org.freedesktop.NetworkManager");
        NetworkManager.DBus.NetworkManager networkManager = nmService.CreateNetworkManager("/org/freedesktop/NetworkManager");
        ObjectPath wlan0Path = await networkManager.GetDeviceByIpIfaceAsync("wlan0");

        try
        {
            await networkManager.ActivateConnectionAsync(
                HotspotPath,
                wlan0Path,
                null!
            );
            _logger.LogInformation("Hotspot activated. Connect to it from another device.");
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Failed to activate hotspot: {ex.Message}");
        }
    }
}