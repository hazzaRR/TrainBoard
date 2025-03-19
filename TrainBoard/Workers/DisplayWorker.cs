using RPiRgbLEDMatrix;
using Microsoft.Extensions.Caching.Memory;
using TrainBoard.Entities;
using TrainBoard.Services;

namespace TrainBoard.Workers;

public class DisplayWorker : BackgroundService
{
    private readonly ILogger<DisplayWorker> _logger;
    private readonly IRgbMatrixService _matrixService;
    private readonly IPlatformEtdService _platformEtdService;
    private readonly IMemoryCache _cache;


    public DisplayWorker(ILogger<DisplayWorker> logger, IRgbMatrixService matrixService, IPlatformEtdService platformEtdService, IMemoryCache cache)
    {
        _logger = logger;
        _matrixService = matrixService;
        _platformEtdService = platformEtdService;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        int scrollTextPos = _matrixService.Canvas.Width;

        while(!_cache.TryGetValue("departureBoard", out ScreenData data)) 
        {
            await Task.Delay(1000, stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised)
            {

                _cache.TryGetValue("departureBoard", out ScreenData data);

                _matrixService.Canvas.Clear();

                _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, new Color(255, 160, 0), data.Std);


                if (_platformEtdService.ShowPlatform)
                {
                    int posFromEndEtd = _matrixService.Canvas.Width - (data.Platform.Length*_matrixService.FontWidth);
                    _matrixService.Canvas.DrawText(_matrixService.Font, posFromEndEtd, _matrixService.FontHeight, new Color(255, 160, 0), data.Platform);
                }
                else 
                {
                    int posFromEndEtd = _matrixService.Canvas.Width - (data.Etd.Length*_matrixService.FontWidth);
                    Color colourToDisplay = data.Etd == "On Time" ? new Color(0, 255, 0) : new Color(255, 15, 0);
                    _matrixService.Canvas.DrawText(_matrixService.Font, posFromEndEtd, _matrixService.FontHeight, colourToDisplay, data.Etd);
                }


                _matrixService.Canvas.DrawText(_matrixService.Font, 0, 14, new Color(255, 160, 0), data.Destination);

                int pixelsDrawn = _matrixService.Canvas.DrawText(_matrixService.Font, scrollTextPos, 22, new Color(255, 160, 0), data.CallingPoints);

                scrollTextPos -= 1;

                if (scrollTextPos + pixelsDrawn < 0)
                {
                    scrollTextPos = _matrixService.Canvas.Width;
                    await Task.Delay(1000, stoppingToken);
                    _cache.TryGetValue("departureBoard", out data);
                }


                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                int timeStartingPos = (_matrixService.Canvas.Width - currentTime.Length*_matrixService.FontWidth) / 2;
                _matrixService.Canvas.DrawText(_matrixService.Font, timeStartingPos, _matrixService.Canvas.Height-1, new Color(255, 160, 0), currentTime);


            _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);



            }

            await Task.Delay(40, stoppingToken);
        }
    }
}
