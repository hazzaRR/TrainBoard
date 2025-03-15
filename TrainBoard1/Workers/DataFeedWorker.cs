using TrainBoard.Services;
using TrainBoard.Entities;
// using OpenLDBWS;
// using OpenLDBWS.Entities;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace TrainBoard.Workers;

public class DataFeedWorker : BackgroundService
{
    private readonly ILogger<DataFeedWorker> _logger;

    private readonly IMemoryCache _cache;
    private readonly IRgbMatrixService _matrixService;
    // private readonly LdbwsClient _client;

    public DataFeedWorker(ILogger<DataFeedWorker> logger, IRgbMatrixService matrixService, IMemoryCache cache)
    {
        _logger = logger;
        _matrixService = matrixService;
        _cache = cache;
        // _client = new("<Api-key>");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised)
            {

                //read this in from a config file

                string departureStationCode = "COL";
                string destinationStationCode = "LST";


                // GetDepBoardWithDetailsResponse response = await _client.GetDepBoardWithDetails(1, departureStationCode, destinationStationCode);

                // List<ServiceWithCallingPoints> services = response.StationBoardWithDetails.TrainServices;


                // for (int i = 0; i < services[0].SubsequentCallingPoints.CallingPoints.Count; i++)
                // {
                //     callingPoints.Append($" {services[0].SubsequentCallingPoints.CallingPoints[i].LocationName} ({services[0].SubsequentCallingPoints.CallingPoints[i].St})");

                //     if (i < services[0].SubsequentCallingPoints.CallingPoints.Count-1)
                //     {
                //         callingPoints.Append(',');
                //     }
                // }


                // ScreenData service = new()
                // {
                //     Std = services[0].Std,
                //     Platform = services[0].Platform,
                //     Destination = services[0].Destination[0].LocationName,
                //     CallingPoints = callingPoints.ToString()
                // };
                ScreenData service = new()
                {
                    Etd = "16:40:00",
                    Std = "16:40:00",
                    Platform = $"Platform {2}",
                    Destination = "London Liv St.",
                    CallingPoints = "CALLING AT: adasdsddsdadasdasd"
                };


                 _cache.Set("departureBoard", service);

            }

            // if (_logger.IsEnabled(LogLevel.Information))
            // {
            //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // }

            await Task.Delay(30000, stoppingToken);
        }
    }
}
