using OpenLDBWS;
using OpenLDBWS.Entities;
namespace TrainBoard.Workers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private LdbwsClient _client;


    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _client = new("<api-key>");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string departureStationCode = "COL";
            string destinationStationCode = "LST";


            GetDepBoardWithDetailsResponse response = await _client.GetDepBoardWithDetails(2, departureStationCode, destinationStationCode);

            Console.WriteLine($"Station Board for: {response.StationBoardWithDetails.LocationName}({response.StationBoardWithDetails.Crs})");
            Console.WriteLine($"For Services to: {response.StationBoardWithDetails.FilterLocationName}({response.StationBoardWithDetails.FilterCrs})");
            Console.WriteLine($"Platform available: {response.StationBoardWithDetails.PlatformAvailable}");
            Console.WriteLine($"Generated at:{response.StationBoardWithDetails.GeneratedAt}");
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(30000, stoppingToken);
        }
    }
}
