using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using TrainBoard.Entities;

namespace TrainBoard.Services;

public interface INetworkConnectivityService
{
    bool IsOnline { get; set; }
    ObjectPath hotspotPath { get; set; }
    Dictionary<string, AvailableNetwork> AvailableNetworks { get; set; }
    Task<bool> IsInternetConnected(int retries, TimeSpan delay);
    Task AddNewConnection(NetworkManagerService nmService, Settings settingsService, NetworkManager.DBus.NetworkManager networkManager, string ssid, string password, ObjectPath wirelessDevicePath, ObjectPath apPath);
}