using TrainBoard.Services;
using Microsoft.Extensions.Caching.Memory;
using RPiRgbLEDMatrix;

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

            if (_matrixService.IsInitialised)
            {
                string currentTime = $"{TimeOnly.FromDateTime(DateTime.Now)}";
                int timeStartingPos = (_matrixService.Canvas.Width - currentTime.Length) / 2;
                _matrixService.Canvas.SetPixels(0, 18, _matrixService.Canvas.Width, 6, (Span<Color>) new Color(0,0,0));
                _matrixService.Canvas.DrawText(_matrixService.Font, timeStartingPos, 16, new Color(255, 160, 0), currentTime);


            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
