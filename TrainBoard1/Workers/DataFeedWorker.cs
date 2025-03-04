using TrainBoard.Services;
using OpenLDBWS;
using OpenLDBWS.Entities;

namespace TrainBoard.Workers;

public class DataFeedWorker : BackgroundService
{
    private readonly ILogger<DataFeedWorker> _logger;

    private readonly IMemoryCache _cache;
    private readonly IRgbMatrixService _matrixService;

    public DataFeedWorker(ILogger<DataFeedWorker> logger, IRgbMatrixService matrixService, IMemoryCache cache)
    {
        _logger = logger;
        _matrixService = matrixService;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised)
            {

                string departureStationCode = "COL";
                string destinationStationCode = "LST";

                LdbwsClient client = new("<api-key>");

                GetDepBoardWithDetailsResponse response = await client.GetDepBoardWithDetails(1, departureStationCode, destinationStationCode);

                //need std, platform and Etd
                // destination
                //calling points Name and then std,

                
                 _cache.Set("departureBoard", response);

            }

            // if (_logger.IsEnabled(LogLevel.Information))
            // {
            //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // }

            await Task.Delay(30000, stoppingToken);
        }
    }
}
