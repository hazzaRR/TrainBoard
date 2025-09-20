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

        while (_matrixService.IsInPairingMode)
        {
            DisplayPairingMode();
            _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);
            await Task.Delay(5000, stoppingToken);
        }

        while (!_matrixService.IsApiKeyValid)
        {
            DisplayInvalidApiKey();
            _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);
            await Task.Delay(5000, stoppingToken);
        }

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
            if (_matrixService.IsInitialised && _matrixService.IsInPairingMode)
            {
                _matrixService.Canvas.Clear();
                DisplayPairingMode();
                _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);
                await Task.Delay(1000, stoppingToken);
            }
            if (_matrixService.IsInitialised && !_matrixService.IsApiKeyValid)
            {
                _matrixService.Canvas.Clear();
                DisplayInvalidApiKey();
                _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);
                await Task.Delay(1000, stoppingToken);
            }
            else if (_matrixService.IsInitialised && !_matrixService.ShowCustomDisplay && data != null)
            {
                if (_callingPointService.IsScrollComplete)
                {
                    _cache.TryGetValue("departureBoard", out data);
                    _callingPointService.IsScrollComplete = false;
                    _callingPointService.showDelayReason = data.DelayReason?.Length > 0 && !_callingPointService.showDelayReason;
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
                await Task.Delay(25, stoppingToken);
            }
            else if (_matrixService.IsInitialised && _matrixService.ShowCustomDisplay)
            {
                _matrixService.Canvas.Clear();
                _matrixService.Canvas.SetPixels(0, 0, _matrixService.Canvas.Width, _matrixService.Canvas.Height, _matrixService.MatrixPixels.AsSpan());
                _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);
                await Task.Delay(1000, stoppingToken);
            }
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

    private void DisplayPairingMode()
    {
        string line1 = "Pairing Mode";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, _matrixService.DestinationColour, line1);

        string line2 = "WiFi: BRboard";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 13, _matrixService.DestinationColour, line2);

        string line3 = "PW: train2go!";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 19, _matrixService.DestinationColour, line3);

        string line4 = "URL:";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 25, _matrixService.DestinationColour, line4);

        string line5 = "trainboard.local";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 31, _matrixService.DestinationColour, line5);
    }
    
    private void DisplayInvalidApiKey()
    {
        string line1 = "Invalid Api key";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, _matrixService.DestinationColour, line1);

        string line2 = "Go to Data feed";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 13, _matrixService.DestinationColour, line2);

        string line3 = "on";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 19, _matrixService.DestinationColour, line3);

        string line4 = "trainboard.local";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 25, _matrixService.DestinationColour, line4);
    }

    private void DisplayDepartureService()
    {
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, _matrixService.StdColour, data.Std);

        if (_platformEtdService.ShowPlatform && (bool)data.IsBusReplacement)
        {
            int posFromEndEtd = _matrixService.Canvas.Width - (9 * _matrixService.FontWidth);
            _matrixService.Canvas.DrawText(_matrixService.Font, posFromEndEtd, _matrixService.FontHeight, _matrixService.PlatformColour, "Rail Repl");
        }
        else if (_platformEtdService.ShowPlatform)
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
        else if (_callingPointService.showDelayReason)
        {
            _callingPointService.PixelsDrawn = _matrixService.Canvas.DrawText(_matrixService.Font, _callingPointService.ScrollTextPos, 22, _matrixService.CallingPointsColour, data.DelayReason);
        }
        else
        {
            _callingPointService.PixelsDrawn = _matrixService.Canvas.DrawText(_matrixService.Font, _callingPointService.ScrollTextPos, 22, _matrixService.CallingPointsColour, data.CallingPoints);
        }
    }
}
