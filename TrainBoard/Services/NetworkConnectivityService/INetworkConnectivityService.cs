using Tmds.DBus.Protocol;
using TrainBoard.Entities;

namespace TrainBoard.Services;

public interface INetworkConnectivityService
{
    bool IsOnline { get; set; }
    ObjectPath HotspotPath { get; set; }
    Dictionary<string, AvailableNetwork> AvailableNetworks { get; set; }
    Task InitialiseNetworkManager(CancellationToken stoppingToken = default);
    Task IsInternetConnected(int retries, TimeSpan delay, CancellationToken stoppingToken = default);
    Task<string> AddNewConnection(string ssid, string password, ObjectPath apPath, CancellationToken stoppingToken = default);
    Task GetSavedConnections(CancellationToken stoppingToken = default);
    Task GetAvailableNetworks(CancellationToken stoppingToken = default);
    Task JoinSavedNetwork(ObjectPath savedConnPath, ObjectPath savedApPath, CancellationToken stoppingToken = default);
    Task EnableHotspot(CancellationToken stoppingToken = default);
}