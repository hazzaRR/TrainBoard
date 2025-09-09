using Tmds.DBus.Protocol;
using TrainBoard.Entities;

namespace TrainBoard.Services;

public interface INetworkConnectivityService
{
    bool IsOnline { get; set; }
    ObjectPath HotspotPath { get; set; }
    Dictionary<string, AvailableNetwork> AvailableNetworks { get; set; }
    Task InitialiseNetworkManager();
    Task IsInternetConnected(int retries, TimeSpan delay);
    Task AddNewConnection(string ssid, string password, ObjectPath apPath);
    Task GetSavedConnections();
    Task GetAvailableNetworks();
    Task JoinSavedNetwork(ObjectPath savedConnPath);
    Task EnableHotspot();
}