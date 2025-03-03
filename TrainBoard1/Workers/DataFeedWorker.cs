using TrainBoard.Services;

namespace TrainBoard.Workers;

public class DataFeedWorker : BackgroundService
{
    private readonly ILogger<DataFeedWorker> _logger;
    private readonly IRgbMatrixService _matrixService;

    public DataFeedWorker(ILogger<DataFeedWorker> logger, IRgbMatrixService matrixService)
    {
        _logger = logger;
        _matrixService = matrixService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised)
            {

            }

            // if (_logger.IsEnabled(LogLevel.Information))
            // {
            //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // }

            await Task.Delay(30000, stoppingToken);
        }
    }
}
