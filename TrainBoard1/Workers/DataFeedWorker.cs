using TrainBoard.Services;
using RPiRgbLEDMatrix;

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

                int timeStartingPos = (_matrixService.Canvas.Width - time.Count) / 2;

                _matrixService.Canvas.SetPixels(0, 18, _matrixService.Canvas.Width, 6, new Color(0,0,0));

                _matrixService.Canvas.DrawText(_matrixService.Font, timeStartingPos, 16, new Color(255, 160, 0), time);


            }

            // if (_logger.IsEnabled(LogLevel.Information))
            // {
            //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // }

            await Task.Delay(30000, stoppingToken);
        }
    }
}
