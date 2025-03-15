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

                Color[] area = new Color[_matrixService.Canvas.Width * _matrixService.FontHeight];
                Array.Fill(area, new Color(0,0,0));

                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                int timeStartingPos = (_matrixService.Canvas.Width - currentTime.Length*_matrixService.FontWidth) / 2;
                _matrixService.Canvas.SetPixels(0, _matrixService.Canvas.Height-_matrixService.FontHeight, _matrixService.Canvas.Width, _matrixService.FontHeight, area);
                _matrixService.Canvas.DrawText(_matrixService.Font, timeStartingPos, _matrixService.Canvas.Height, new Color(255, 160, 0), currentTime);

                _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
