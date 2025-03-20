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

                for (int i = 0; i < services[0].SubsequentCallingPoints.CallingPoints.Count; i++)
                {
                    callingPoints.Add($" {services[0].SubsequentCallingPoints.CallingPoints[i].LocationName} ({services[0].SubsequentCallingPoints.CallingPoints[i].St})");
                }

                string destination = "";
                stationAliases.TryGetValue(services[0].Destination[0].LocationName, out destination);

                ScreenData service = new()
                {
                    Std = services[0].Std,
                    Etd = services[0].Etd.Equals("On time", StringComparison.CurrentCultureIgnoreCase) ||
                    services[0].Etd.Equals("Cancelled", StringComparison.CurrentCultureIgnoreCase) ||
                    services[0].Etd.Equals("Delayed", StringComparison.CurrentCultureIgnoreCase) ? 
                    textInfo.ToTitleCase(services[0].Etd) : $"Expt.{services[0].Etd}",
                    Platform = $"Plat {services[0].Platform}",
                    Destination = !string.IsNullOrEmpty(destination) ? destination : services[0].Destination[0].LocationName,
                    CallingPoints = string.Join(",", callingPoints),
                    IsCancelled = services[0].IsCancelled,
                    DelayReason = services[0].DelayReason,
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
