using RPiRgbLEDMatrix;
using Microsoft.Extensions.Caching.Memory;
using TrainBoard.Entities;
using TrainBoard.Services;

namespace TrainBoard.Workers;

public class DisplayWorker : BackgroundService
{
    private readonly IRgbMatrixService _matrixService;
    private readonly ICallingPointService _callingPointService;
    private readonly IPlatformEtdService _platformEtdService;
    private readonly IDestinationService _destinationService;
    private readonly IMemoryCache _cache;
    private ScreenData? data;


    public DisplayWorker(IRgbMatrixService matrixService, IPlatformEtdService platformEtdService, ICallingPointService callingPointService, IDestinationService destinationService, IMemoryCache cache)
    {
        _matrixService = matrixService;
        _callingPointService = callingPointService;
        _platformEtdService = platformEtdService;
        _destinationService = destinationService;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!_cache.TryGetValue("departureBoard", out data))
        {
            await Task.Delay(1000, stoppingToken);
        }

        if (!data.NoServices)
        {
            _destinationService.DestinationWidthInPixels = !data.NoServices ? data.Destination.Length * _matrixService.FontWidth : 0;
            _destinationService.IsDestinationScrollable = _destinationService.DestinationWidthInPixels > _matrixService.Canvas.Width;
        }

        while (!stoppingToken.IsCancellationRequested)
        {

            if (_matrixService.IsInitialised && !_matrixService.ShowCustomDisplay && data != null)
            {
                if (_callingPointService.IsScrollComplete)
                {
                    _cache.TryGetValue("departureBoard", out data);
                    _callingPointService.IsScrollComplete = false;
                    _destinationService.ScrollTextPos = 0;
                    _destinationService.DestinationWidthInPixels = !data.NoServices ? data.Destination.Length * _matrixService.FontWidth : 0;
                    _destinationService.IsDestinationScrollable = _destinationService.DestinationWidthInPixels > _matrixService.Canvas.Width;
                }

                _matrixService.Canvas.Clear();

                if (data.NoServices)
                {
                    DisplayNoServices();
                }
                else
                {
                    DisplayDepartureService();
                }

                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                int timeStartingPos = (_matrixService.Canvas.Width - currentTime.Length * _matrixService.FontWidth) / 2;
                _matrixService.Canvas.DrawText(_matrixService.Font, timeStartingPos, _matrixService.Canvas.Height - 1, _matrixService.CurrentTimeColour, currentTime);

                _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);
            }
            else if (_matrixService.IsInitialised && _matrixService.ShowCustomDisplay)
            {
                _matrixService.Canvas.SetPixels(0, 0, _matrixService.Canvas.Width, _matrixService.Canvas.Height, _matrixService.MatrixPixels.AsSpan());
            }

            await Task.Delay(10, stoppingToken);
        }
    }

    private void DisplayNoServices()
    {
        string line1 = "There are";
        int line1StartingPos = (_matrixService.Canvas.Width - line1.Length * _matrixService.FontWidth) / 2;
        _matrixService.Canvas.DrawText(_matrixService.Font, line1StartingPos, _matrixService.FontHeight, _matrixService.DestinationColour, line1);

        string line2 = "currently no";
        int line2StartingPos = (_matrixService.Canvas.Width - line2.Length * _matrixService.FontWidth) / 2;
        _matrixService.Canvas.DrawText(_matrixService.Font, line2StartingPos, 12, _matrixService.DestinationColour, line2);

        string line3 = "services";
        int line3StartingPos = (_matrixService.Canvas.Width - line3.Length * _matrixService.FontWidth) / 2;
        _matrixService.Canvas.DrawText(_matrixService.Font, line3StartingPos, 18, _matrixService.DestinationColour, line3);
    }

    private void DisplayDepartureService()
    {
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, _matrixService.StdColour, data.Std);

        if (_platformEtdService.ShowPlatform)
        {
            int posFromEndEtd = _matrixService.Canvas.Width - (data.Platform.Length * _matrixService.FontWidth);
            _matrixService.Canvas.DrawText(_matrixService.Font, posFromEndEtd, _matrixService.FontHeight, _matrixService.PlatformColour, data.Platform);
        }
        else
        {
            int posFromEndEtd = _matrixService.Canvas.Width - (data.Etd.Length * _matrixService.FontWidth);
            Color colourToDisplay = data.Etd == "On Time" ? _matrixService.OnTimeColour : _matrixService.DelayColour;
            _matrixService.Canvas.DrawText(_matrixService.Font, posFromEndEtd, _matrixService.FontHeight, colourToDisplay, data.Etd);
        }

        _matrixService.Canvas.DrawText(_matrixService.Font, _destinationService.ScrollTextPos, 14, _matrixService.DestinationColour, data.Destination);

        if ((bool)data.IsCancelled && !string.IsNullOrEmpty(data.CancelReason))
        {
            _callingPointService.PixelsDrawn = _matrixService.Canvas.DrawText(_matrixService.Font, _callingPointService.ScrollTextPos, 22, _matrixService.CallingPointsColour, data.CancelReason);
        }
        else
        {
            _callingPointService.PixelsDrawn = _matrixService.Canvas.DrawText(_matrixService.Font, _callingPointService.ScrollTextPos, 22, _matrixService.CallingPointsColour, data.CallingPoints);
        }
    }
}
