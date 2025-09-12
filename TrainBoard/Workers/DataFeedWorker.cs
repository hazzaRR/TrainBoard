using TrainBoard.Services;
using TrainBoard.Entities;
using OpenLDBWS;
using OpenLDBWS.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using MQTTnet;
using System.Text.Json;

namespace TrainBoard.Workers;

public class DataFeedWorker : BackgroundService
{
    private readonly ILogger<DataFeedWorker> _logger;
    private readonly IMemoryCache _cache;
    private readonly IRgbMatrixService _matrixService;
    private readonly INetworkConnectivityService _networkConnectivityService;
    private readonly ILdbwsClient _client;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _options;
    private RgbMatrixConfiguration _config;
    private Dictionary<string, string> _stationAliases;
    private TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
    private JsonSerializerOptions serializeOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public DataFeedWorker(ILogger<DataFeedWorker> logger, IRgbMatrixService matrixService, INetworkConnectivityService networkConnectivityService, IMemoryCache cache, ILdbwsClient client)
    {
        _logger = logger;
        _matrixService = matrixService;
        _networkConnectivityService = networkConnectivityService;
        _cache = cache;
        _client = client;

        _mqttClient = new MqttClientFactory().CreateMqttClient();

        _options = new MqttClientOptionsBuilder()
        .WithTcpServer("localhost", 1883)
        .WithClientId("matrixWorker")
        .WithCleanSession()
        .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        string? matrixSettings = await File.ReadAllTextAsync("./matrixSettings.json", stoppingToken);
        _logger.LogInformation($"current matrix settings: {matrixSettings}");

        if (string.IsNullOrEmpty(matrixSettings))
        {
            _config = new();
            _logger.LogInformation($"No matrix settings found, writing default settings to file");
            await File.WriteAllTextAsync("./matrixSettings.json", JsonSerializer.Serialize(_config, serializeOptions), stoppingToken);
        }
        else
        {
            _config = JsonSerializer.Deserialize<RgbMatrixConfiguration>(matrixSettings, serializeOptions);
        }
        _matrixService.SetUserOptions(_config);

        SetupMqttEventHandlers(stoppingToken);
        await _mqttClient.ConnectAsync(_options, stoppingToken);
        await PublishConfig("matrix/config", _config, stoppingToken);
        _stationAliases = new()
        {
            {"London Liverpool Street", "London Liv St."}
        };

        await _networkConnectivityService.IsInternetConnected(6, TimeSpan.FromSeconds(5));
        await _networkConnectivityService.InitialiseNetworkManager();
        await _networkConnectivityService.GetSavedConnections();
        await _networkConnectivityService.GetAvailableNetworks();

        await PublishConfig("network/available", _networkConnectivityService.AvailableNetworks, stoppingToken);

        if (!_networkConnectivityService.IsOnline)
        {
            await _networkConnectivityService.EnableHotspot();
            _matrixService.IsInParingMode = true;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_matrixService.IsInitialised && _networkConnectivityService.IsOnline)
            {
                await GetNewDepartureBoardDetails(stoppingToken);
                await Task.Delay(30000, stoppingToken);
            }
            else
            {
                await Task.Delay(5000, stoppingToken);
            }

        }
    }

    private void SetupMqttEventHandlers(CancellationToken stoppingToken)
    {

        _mqttClient.ConnectedAsync += async e => 
        {
            await _mqttClient.SubscribeAsync("matrix/config", cancellationToken: stoppingToken);
            await _mqttClient.SubscribeAsync("network/manage", cancellationToken: stoppingToken);
        };

        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            if (e.ApplicationMessage.Topic.Equals("matrix/config"))
            {
                _config = JsonSerializer.Deserialize<RgbMatrixConfiguration>(e.ApplicationMessage.ConvertPayloadToString(), serializeOptions);
                _matrixService.SetUserOptions(_config);
                _logger.LogInformation($"New config recieved: {_config}");
                try
                {
                    await File.WriteAllTextAsync("./matrixSettings.json", e.ApplicationMessage.ConvertPayloadToString(), stoppingToken);
                    await GetNewDepartureBoardDetails(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex}");
                }
            }
            else if (e.ApplicationMessage.Topic.Equals("network/manage"))
            {
                NewConnection newConnection = JsonSerializer.Deserialize<NewConnection>(e.ApplicationMessage.ConvertPayloadToString(), serializeOptions);
                _logger.LogInformation($"New connection config recieved: {newConnection}");
                try
                {
                    _networkConnectivityService.AvailableNetworks.TryGetValue(newConnection.Key, out var apConnection);
                    if (newConnection.UseSaved)
                    {
                        await _networkConnectivityService.JoinSavedNetwork(apConnection.ApPath.Value!);
                    }
                    else
                    {
                        _networkConnectivityService.AddNewConnection(apConnection.Ssid, newConnection.Password, apConnection.ApPath.Value!);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex}");
                }
            }
        };

        _mqttClient.DisconnectedAsync += async e => 
        {
            await Task.Delay(5000);

            try
            {
                await _mqttClient.ConnectAsync(_options, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reconnection failed: {ex.Message}");
            }
        };

    }

    private async Task GetNewDepartureBoardDetails(CancellationToken stoppingToken)
    {
        GetDepBoardWithDetailsResponse response = await _client.GetDepBoardWithDetails(_config.NumRows, _config.Crs, _config.FilterCrs, _config.FilterType, _config.TimeOffset, _config.TimeWindow);

        List<ServiceWithCallingPoints> services = response.StationBoardWithDetails.TrainServices;
        List<string> callingPoints = new List<string>();
        ScreenData service;

        ServiceWithCallingPoints? nextService = services.FirstOrDefault();

        if (nextService != null)
        {
            for (int i = 0; i < nextService.SubsequentCallingPoints.CallingPoints.Count; i++)
            {
                callingPoints.Add($" {nextService.SubsequentCallingPoints.CallingPoints[i].LocationName} ({nextService.SubsequentCallingPoints.CallingPoints[i].St})");
            }

            string destination = "";
            _stationAliases.TryGetValue(nextService.Destination[0].LocationName, out destination);

            service = new()
            {
                Std = nextService.Std,
                Etd = nextService.Etd.Equals("On time", StringComparison.CurrentCultureIgnoreCase) ||
                nextService.Etd.Equals("Cancelled", StringComparison.CurrentCultureIgnoreCase) ||
                nextService.Etd.Equals("Delayed", StringComparison.CurrentCultureIgnoreCase) ? 
                textInfo.ToTitleCase(nextService.Etd) : $"Exp.{nextService.Etd}",
                Platform = $"Plat {nextService.Platform}",
                Destination = !string.IsNullOrEmpty(destination) ? destination : nextService.Destination[0].LocationName,
                CallingPoints = string.Join(",", callingPoints),
                IsCancelled = nextService.IsCancelled,
                DelayReason = nextService.DelayReason,
            };

        }
        else 
        {
            service = new()
            {
                NoServices = true
            };
        }

        _cache.Set("departureBoard", service);
    }

    private async Task PublishConfig<T>(string topic, T payload, CancellationToken stoppingToken)
    {
        try
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(JsonSerializer.Serialize(payload, serializeOptions))
            .WithRetainFlag()
            .Build();

            await _mqttClient.PublishAsync(applicationMessage, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Connection failed: {ex.Message}");
        }
    }
}