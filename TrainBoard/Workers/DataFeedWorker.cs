using TrainBoard.Services;
using TrainBoard.Entities;
using OpenLDBWS;
using OpenLDBWS.Entities;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace TrainBoard.Workers;

public class DataFeedWorker : BackgroundService
{
    private readonly ILogger<DataFeedWorker> _logger;
    private readonly IMemoryCache _cache;
    private readonly IRgbMatrixService _matrixService;
    private readonly ILdbwsClient _client;

    public DataFeedWorker(ILogger<DataFeedWorker> logger, IRgbMatrixService matrixService, IMemoryCache cache, ILdbwsClient client)
    {
        _logger = logger;
        _matrixService = matrixService;
        _cache = cache;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        Dictionary<string, string> stationAliases = new ()
        {
            {"London Liverpool Street", "London Liv St."}
        };

        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised)
            {

                //read this in from a config file

                string departureStationCode = "COL";
                string destinationStationCode = "LST";


                GetDepBoardWithDetailsResponse response = await _client.GetDepBoardWithDetails(1, departureStationCode, destinationStationCode);

                List<ServiceWithCallingPoints> services = response.StationBoardWithDetails.TrainServices;


                List<string> callingPoints = new List<string>();

                for (int i = 0; i < services[0].SubsequentCallingPoints.CallingPoints.Count; i++)
                {
                    callingPoints.Add($" {services[0].SubsequentCallingPoints.CallingPoints[i].LocationName} ({services[0].SubsequentCallingPoints.CallingPoints[i].St})");
                }

                string destination = "";
                stationAliases.TryGetValue(services[0].Destination[0].LocationName, out destination);

                ScreenData service = new()
                {
                    Std = services[0].Std,
                    Etd = services[0].Etd == "On time" ? "On Time" : $"Exp. {services[0].Etd}",
                    Platform = $"Plat {services[0].Platform}",
                    Destination = destination != "" ? destination : services[0].Destination[0].LocationName,
                    CallingPoints = string.Join(",", callingPoints)
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
