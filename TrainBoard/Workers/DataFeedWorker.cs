using TrainBoard.Services;
using TrainBoard.Entities;
using OpenLDBWS;
using OpenLDBWS.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Globalization;

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

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

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
}
