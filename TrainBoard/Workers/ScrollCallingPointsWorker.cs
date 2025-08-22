using TrainBoard.Services;

namespace TrainBoard.Workers;

public class ScrollCallingPointsWorker : BackgroundService
{
    private readonly ICallingPointService _callingPointService;
    private readonly IRgbMatrixService _matrixService;
    public ScrollCallingPointsWorker(ICallingPointService callingPointService, IRgbMatrixService matrixService)
    {
        _callingPointService = callingPointService;
        _matrixService = matrixService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

         _callingPointService.ScrollTextPos = _matrixService.Canvas.Width;
        while (!stoppingToken.IsCancellationRequested)
        {

                _callingPointService.ScrollTextPos -= 1;

                if (_callingPointService.ScrollTextPos + _callingPointService.PixelsDrawn < 0)
                {
                    _callingPointService.ScrollTextPos = _matrixService.Canvas.Width;
                    _callingPointService.IsScrollComplete = true;
                    await Task.Delay(1000, stoppingToken);
                }
            await Task.Delay(60, stoppingToken);
        }
    }
}
