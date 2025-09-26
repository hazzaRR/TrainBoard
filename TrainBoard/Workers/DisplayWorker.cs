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
    private int _timeout = 1000;
    private int frame = -1;


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
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_matrixService.IsInitialised)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            _matrixService.Canvas.Clear();

            if (_matrixService.IsInPairingMode)
            {
                DisplayPairingMode();
                _timeout = 2000;
            }
            else if (_matrixService.IsApiKeyInvalid)
            {
                DisplayInvalidApiKey();
                _timeout = 2000;
            }
            else if (_matrixService.ShowCustomDisplay)
            {
                frame = (frame + 1) % _matrixService.MatrixFrames.Length;
                _matrixService.Canvas.SetPixels(0, 0, _matrixService.Canvas.Width, _matrixService.Canvas.Height, _matrixService.MatrixFrames[frame].Pixels.AsSpan());
                _timeout = _matrixService.MatrixFrames[frame].Delay;
            }
            else if (data == null)
            {
                _cache.TryGetValue("departureBoard", out data);
                string line = "Loading...";
                int lineStartingPos = (_matrixService.Canvas.Width - line.Length * _matrixService.FontWidth) / 2;
                _matrixService.Canvas.DrawText(_matrixService.Font, lineStartingPos, _matrixService.Canvas.Height / 2, _matrixService.DestinationColour, line);
                _timeout = 1000;
            }
            else
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
                _timeout = 25;
            }
            
            _matrixService.Matrix.SwapOnVsync(_matrixService.Canvas);
            await Task.Delay(_timeout, stoppingToken);
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
        string line1 = "PAIRING MODE";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, _matrixService.DestinationColour, line1);

        string line2 = "WIFI: BRboard";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 13, _matrixService.DestinationColour, line2);

        string line3 = "PW: train2go!";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 19, _matrixService.DestinationColour, line3);

        string line4 = "GO TO:";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 25, _matrixService.DestinationColour, line4);

        string line5 = "TRAINBOARD.LOCAL";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 31, _matrixService.DestinationColour, line5);
    }
    
    private void DisplayInvalidApiKey()
    {
        string line1 = "INVALID API KEY";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, _matrixService.FontHeight, _matrixService.DestinationColour, line1);

        string line2 = "GO TO:";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 13, _matrixService.DestinationColour, line2);

        string line3 = "TRAINBOARD.LOCAL";
        _matrixService.Canvas.DrawText(_matrixService.Font, 0, 19, _matrixService.DestinationColour, line3);

        string line4 = "DATA FEED PAGE";
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
