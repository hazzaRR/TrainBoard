using RPiRgbLEDMatrix;
using Microsoft.Extensions.Caching.Memory;
using TrainBoard.Entities;
using TrainBoard.Services;

namespace TrainBoard.Workers;

public class DisplayWorker : BackgroundService
{
    private readonly ILogger<DisplayWorker> _logger;
    private readonly IRgbMatrixService _matrixService;
    private readonly IPlatformStdService _platformStdService;
    private readonly ICallingPointService _callingPointService;
    private readonly IMemoryCache _cache;
    private ScreenData data;


    public DisplayWorker(ILogger<DisplayWorker> logger, IRgbMatrixService matrixService, IPlatformStdService platformStdService, ICallingPointService callingPointService, IMemoryCache cache)
    {
        _logger = logger;
        _matrixService = matrixService;
        _platformStdService = platformStdService;
        _callingPointService = callingPointService;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        int scrollTextPos = _matrixService.Canvas.Width;

        while(!_cache.TryGetValue("departureBoard", out data)) 
        {
            await Task.Delay(1000, stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised && data != null)
            {

                if (_callingPointService.IsScrollComplete)
                {
                    _cache.TryGetValue("departureBoard", out data);
                    _callingPointService.IsScrollComplete = false;
                }

                _matrixService.Canvas.Clear();

                if (_platformStdService.ShowPlatform)
                {
                    _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, new Color(255, 160, 0), data.Platform);
                }
                else 
                {
                    _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, new Color(255, 255, 255), data.Std);
                }

                int posFromEndEtd = _matrixService.Canvas.Width - (data.Etd.Length*_matrixService.FontWidth);
                Color colourToDisplay = data.Etd == "On Time" ? new Color(0, 255, 0) : new Color(255, 15, 0);
                _matrixService.Canvas.DrawText(_matrixService.Font, posFromEndEtd, _matrixService.FontHeight, colourToDisplay, data.Etd);

                _matrixService.Canvas.DrawText(_matrixService.Font, 0, 14, new Color(255, 160, 0), data.Destination);

                _callingPointService.PixelsDrawn = _matrixService.Canvas.DrawText(_matrixService.Font, _callingPointService.ScrollTextPos, 22, new Color(255, 160, 0), data.CallingPoints);

                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                int timeStartingPos = (_matrixService.Canvas.Width - currentTime.Length*_matrixService.FontWidth) / 2;
                _matrixService.Canvas.DrawText(_matrixService.Font, timeStartingPos, _matrixService.Canvas.Height-1, new Color(255, 160, 0), currentTime);


                _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);

            }

            await Task.Delay(10, stoppingToken);
        }
    }
}
