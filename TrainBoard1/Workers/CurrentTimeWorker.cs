using TrainBoard.Services;

namespace TrainBoard.Workers;

public class CurrentTimeWorker : BackgroundService
{
    private readonly ILogger<CurrentTimeWorker> _logger;
    private readonly IRgbMatrixService _matrixService;

    public CurrentTimeWorker(ILogger<CurrentTimeWorker> logger, IRgbMatrixService matrixService)
    {
        _logger = logger;
        _matrixService = matrixService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            if (!_matrixService.IsInitialised)
            {

            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
