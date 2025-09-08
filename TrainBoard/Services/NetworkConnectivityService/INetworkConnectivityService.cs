using Tmds.DBus.Protocol;
using TrainBoard.Entities;

namespace TrainBoard.Services;

public interface INetworkConnectivityService
{
    bool IsOnline { get; set; }
    ObjectPath HotspotPath { get; set; }
    Dictionary<string, AvailableNetwork> AvailableNetworks { get; set; }
    Task IsInternetConnected(int retries, TimeSpan delay);
    Task AddNewConnection(Connection connection, string ssid, string password, ObjectPath apPath);
    Task GetSavedConnections(Connection connection);
    Task GetAvailableNetworks(Connection connection);
    Task JoinSavedNetwork(Connection connection, ObjectPath savedConnPath);
    Task EnableHotspot(Tmds.DBus.Protocol.Connection connection);
}