using TrainBoard.Services;

namespace TrainBoard.Workers;

public class CallingPointsWorker : BackgroundService
{
    private readonly ILogger<CallingPointsWorker> _logger;
    private readonly IRgbMatrixService _matrixService;

    public CallingPointsWorker(ILogger<CallingPointsWorker> logger, IRgbMatrixService matrixService)
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

            await Task.Delay(5, stoppingToken);
        }
    }
}
