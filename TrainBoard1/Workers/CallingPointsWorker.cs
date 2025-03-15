using RPiRgbLEDMatrix;
using Microsoft.Extensions.Caching.Memory;
using TrainBoard.Entities;
using TrainBoard.Services;

namespace TrainBoard.Workers;

public class CallingPointsWorker : BackgroundService
{
    private readonly ILogger<CallingPointsWorker> _logger;
    private readonly IRgbMatrixService _matrixService;
    private readonly IMemoryCache _cache;


    public CallingPointsWorker(ILogger<CallingPointsWorker> logger, IRgbMatrixService matrixService, IMemoryCache cache)
    {
        _logger = logger;
        _matrixService = matrixService;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        int scrollTextPos = _matrixService.Canvas.Width;
        _cache.TryGetValue("departureBoard", out ScreenData data);

        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised)
            {

                Color[] area = new Color[_matrixService.Canvas.Width * _matrixService.FontHeight];
                Array.Fill(area, new Color(0,0,0));

                _matrixService.Canvas.SetPixels(0, 10, _matrixService.Canvas.Width, _matrixService.FontHeight, area);

                int pixelsDrawn = _matrixService.Canvas.DrawText(_matrixService.Font, scrollTextPos, 16, new Color(255, 160, 0), data.CallingPoints);

                scrollTextPos -= 1;

                if (scrollTextPos + pixelsDrawn < 0)
                {
                    scrollTextPos = _matrixService.Canvas.Width;
                    await Task.Delay(2, stoppingToken);
                    _cache.TryGetValue("departureBoard", out data);
                }


            _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);



            }

            await Task.Delay(20, stoppingToken);
        }
    }
}
