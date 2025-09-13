using System.Text;
using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using TrainBoard.Entities;

namespace TrainBoard.Services;
public class NetworkConnectivityService : INetworkConnectivityService
{

    private readonly ILogger<NetworkConnectivityService> _logger;
    public bool IsOnline { get; set; }
    private Tmds.DBus.Protocol.Connection _connection;
    private NetworkManagerService _nmService;
    private NetworkManager.DBus.NetworkManager _networkManager;
    private ObjectPath _wirelessDevicePath;
    private Settings _settingsService;
    private Wireless _wirelessDevice;
    public ObjectPath HotspotPath { get; set; }
    public Dictionary<string, ObjectPath> SavedConnections { get; set; }
    public Dictionary<string, AvailableNetwork> AvailableNetworks { get; set; } = [];

    public NetworkConnectivityService(ILogger<NetworkConnectivityService> logger)
    {
        _logger = logger;
    }

    public async Task InitialiseNetworkManager()
    {
        _connection = new(Address.System!);
        await _connection.ConnectAsync();
        _nmService = new NetworkManagerService(_connection, "org.freedesktop.NetworkManager");
        _networkManager = _nmService.CreateNetworkManager("/org/freedesktop/NetworkManager");
        _wirelessDevicePath = await _networkManager.GetDeviceByIpIfaceAsync("wlan0");
        _settingsService = _nmService.CreateSettings("/org/freedesktop/NetworkManager/Settings");
        _wirelessDevice = _nmService.CreateWireless(_wirelessDevicePath);
        _logger.LogInformation("Network Manager connections successfully initialised");
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

    public async Task<string> AddNewConnection(string ssid, string password, ObjectPath apPath)
    {
        try
        {
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

            ObjectPath wifiConnPath = await _settingsService.AddConnectionAsync(wifiSettings);
            _logger.LogInformation($"Connection made {wifiConnPath}");
            var activeConn = await _networkManager.ActivateConnectionAsync(
                wifiConnPath,
                _wirelessDevicePath,
                apPath
            );

            _logger.LogInformation($"Connection activated: {activeConn}");

            return $"successfully connected to: {ssid}";

        }
        catch (DBusException ex)
        {
            if (ex.Message.Contains("802-11-wireless-security.psk: property is invalid"))
            {
                _logger.LogError($"Failed to add connection for SSID: {ssid}. An incorrect password was entered.");
                return $"password provided was incorrect";
            }
            else
            {
                _logger.LogError(ex, $"An unexpected D-Bus error occurred while adding connection for SSID: {ssid}.");
                return $"Error connecting to {ssid}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unknown error occurred while adding connection for SSID: {ssid}.");
            return $"Error connecting to {ssid}";
        }
    }

    public async Task GetSavedConnections()
    {
        var savedConnections = new Dictionary<string, ObjectPath>();
        ObjectPath[] allSavedConnections = await _settingsService.ListConnectionsAsync();

        foreach (var connPath in allSavedConnections)
        {
            var conn = _nmService.CreateConnection(connPath);
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

    public async Task GetAvailableNetworks()
    {
        _logger.LogInformation("Scanning for available Wi-Fi networks...");
        await _wirelessDevice.RequestScanAsync(new Dictionary<string, VariantValue>());
        // await Task.Delay(5000); //Is this even need anymore??
        _logger.LogInformation("Scan complete. Retrieving access points...");

        ObjectPath[] availableAccessPointsPaths = await _wirelessDevice.GetAccessPointsAsync();
        ObjectPath activeApPath = await _wirelessDevice.GetActiveAccessPointAsync();

        var combinedNetworks = new Dictionary<string, AvailableNetwork>();

        foreach (var apPath in availableAccessPointsPaths)
        {
            var accessPoint = _nmService.CreateAccessPoint(apPath);

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
    public async Task JoinSavedNetwork(ObjectPath savedConnPath)
    {
        try
        {
            var activeConn = await _networkManager.ActivateConnectionAsync(
                savedConnPath,
                _wirelessDevicePath,
                savedConnPath
            );
            _logger.LogInformation($"Connection activated: {activeConn}");
        }
        catch (DBusException ex)
        {
            _logger.LogError(ex, $"An unexpected D-Bus error occurred while connecting to: {savedConnPath}.");
        }

    }

    public async Task EnableHotspot()
    {
        try
        {
            await _networkManager.ActivateConnectionAsync(
                HotspotPath,
                _wirelessDevicePath,
                null!
            );
            _logger.LogInformation("Hotspot activated. Connect to it from another device.");
        }
        catch (DBusException ex)
        {
            _logger.LogError($"Failed to activate hotspot: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Failed to activate hotspot: {ex.Message}");
        }
    }
}