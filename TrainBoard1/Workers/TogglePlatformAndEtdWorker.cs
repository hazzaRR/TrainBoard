using RPiRgbLEDMatrix;
using TrainBoard.Entities;
using TrainBoard.Services;
using Microsoft.Extensions.Caching.Memory;

namespace TrainBoard.Workers;

public class TogglePlatformAndEtdWorker : BackgroundService
{
    private readonly ILogger<TogglePlatformAndEtdWorker> _logger;
    private readonly IRgbMatrixService _matrixService;
    private readonly IMemoryCache _cache;


    public TogglePlatformAndEtdWorker(ILogger<TogglePlatformAndEtdWorker> logger, IRgbMatrixService matrixService, IMemoryCache cache)
    {
        _logger = logger;
        _matrixService = matrixService;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        bool showEtd = false;
        _cache.TryGetValue("departureBoard", out ScreenData data);

        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised)
            {

                int posFromEndEtd = _matrixService.Canvas.Width - data.Etd.Length;
                int posFromEndPlatfrom = _matrixService.Canvas.Width - data.Platform.Length;

                if (showEtd)
                {
                    Color[] area = new Color[data.Platform.Length * 6];
                    Array.Fill(area, new Color(0,0,0));
                    _matrixService.Canvas.SetPixels(posFromEndPlatfrom, 0, data.Platform.Length, 6, area);

                    Color colourToDisplay = data.Etd == "On Time" ? new Color(0, 255, 0) : new Color(255, 0, 0);
                    _matrixService.Canvas.DrawText(_matrixService.Font, posFromEndEtd, 5, colourToDisplay, data.Etd);

                }
                else
                {
                    Color[] area = new Color[data.Etd.Length * 6];
                    Array.Fill(area, new Color(0,0,0));
                    _matrixService.Canvas.SetPixels(posFromEndEtd, 0, data.Etd.Length, 6, area);
                    _matrixService.Canvas.DrawText(_matrixService.Font, posFromEndPlatfrom, 5, new Color(255, 160, 0), data.Platform);

                }

                Thread.Sleep(10000);
                showEtd = !showEtd;

                _cache.TryGetValue("departureBoard", out data);

            }

            await Task.Delay(5, stoppingToken);
        }
    }
}
