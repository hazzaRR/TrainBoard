using TrainBoard.Services;
using TrainBoard.Entities;
using OpenLDBWS;
using OpenLDBWS.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using MQTTnet;
using System.Text.Json;
using RPiRgbLEDMatrix;
using TrainBoard.Utilities;

namespace TrainBoard.Workers;

public class DataFeedWorker : BackgroundService
{
    private readonly ILogger<DataFeedWorker> _logger;
    private readonly IMemoryCache _cache;
    private readonly IRgbMatrixService _matrixService;
    private readonly ILdbwsClient _client;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _options;
    private RgbMatrixConfiguration _config;

    public DataFeedWorker(ILogger<DataFeedWorker> logger, IRgbMatrixService matrixService, IMemoryCache cache, ILdbwsClient client)
    {
        _logger = logger;
        _matrixService = matrixService;
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
            await File.WriteAllTextAsync("./matrixSettings.json", JsonSerializer.Serialize(_config), stoppingToken);
        }
        else
        {
            _config = JsonSerializer.Deserialize<RgbMatrixConfiguration>(matrixSettings);
        }
        _matrixService.StdColour = ColourConverter.IntToRgb(_config.StdColour);
        _matrixService.PlatformColour = ColourConverter.IntToRgb(_config.PlatformColour);
        _matrixService.DestinationColour = ColourConverter.IntToRgb(_config.DestinationColour);
        _matrixService.CallingPointsColour = ColourConverter.IntToRgb(_config.CallingPointsColour);
        _matrixService.CurrentTimeColour = ColourConverter.IntToRgb(_config.CurrentTimeColour);
        _matrixService.DelayColour = ColourConverter.IntToRgb(_config.DelayColour);
        _matrixService.OnTimeColour = ColourConverter.IntToRgb(_config.OnTimeColour);
        _matrixService.ShowCustomDisplay = _config.ShowCustomDisplay;
        _matrixService.MatrixPixels = Flattern2dColourMatrix(_config.MatrixPixels);

        SetupMqttEventHandlers(stoppingToken);
        await PublishConfig(stoppingToken);
        Dictionary<string, string> stationAliases = new ()
        {
            {"London Liverpool Street", "London Liv St."}
        };

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised)
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
                    stationAliases.TryGetValue(nextService.Destination[0].LocationName, out destination);

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

            // if (_logger.IsEnabled(LogLevel.Information))
            // {
            //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // }

            await Task.Delay(30000, stoppingToken);
        }
    }

    private void SetupMqttEventHandlers(CancellationToken stoppingToken)
    {

        _mqttClient.ConnectedAsync += async e => 
        {
            await _mqttClient.SubscribeAsync("matrix_config", cancellationToken: stoppingToken);
        };

        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            if (e.ApplicationMessage.Topic.Equals("matrix_config"))
            {
                _config = JsonSerializer.Deserialize<RgbMatrixConfiguration>(e.ApplicationMessage.ConvertPayloadToString());
                _matrixService.StdColour = ColourConverter.IntToRgb(_config.StdColour);
                _matrixService.PlatformColour = ColourConverter.IntToRgb(_config.PlatformColour);
                _matrixService.DestinationColour = ColourConverter.IntToRgb(_config.DestinationColour);
                _matrixService.CallingPointsColour = ColourConverter.IntToRgb(_config.CallingPointsColour);
                _matrixService.CurrentTimeColour = ColourConverter.IntToRgb(_config.CurrentTimeColour);
                _matrixService.DelayColour = ColourConverter.IntToRgb( _config.DelayColour);
                _matrixService.OnTimeColour = ColourConverter.IntToRgb(_config.OnTimeColour);
                _matrixService.ShowCustomDisplay = _config.ShowCustomDisplay;
                _matrixService.MatrixPixels = Flattern2dColourMatrix(_config.MatrixPixels);
                _logger.LogInformation($"New config recieved: {_config}");
                try
                {
                    await File.WriteAllTextAsync("./matrixSettings.json", e.ApplicationMessage.ConvertPayloadToString(), stoppingToken);
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

    private async Task PublishConfig(CancellationToken stoppingToken)
    {
        try 
        {
            await _mqttClient.ConnectAsync(_options, stoppingToken);

            var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("matrix_config")
            .WithPayload(JsonSerializer.Serialize(_config))
            .WithRetainFlag()
            .Build();

            await _mqttClient.PublishAsync(applicationMessage, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Connection failed: {ex.Message}");
        }
    }

    private Color[] Flattern2dColourMatrix(int[][]? colourMatrix)
    {
        int rows = 32;
        int cols = 64;

        if (colourMatrix == null || colourMatrix.Length == 0)
        {
            colourMatrix = new int[rows][];
        }

        Color[] colourArray = new Color[rows * cols];

        int pixel = 0;

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (colourMatrix[i] == null || colourMatrix[i].Length == 0)
                {
                    colourMatrix[i] = new int[cols];
                }
                colourArray[pixel] = ColourConverter.IntToRgb(colourMatrix[j][i]);
                pixel++;
            }
        }

        return colourArray;
    }
}