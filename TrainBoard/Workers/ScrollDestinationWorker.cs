using TrainBoard.Services;

namespace TrainBoard.Workers;

public class ScrollDestinationWorker : BackgroundService
{
    private readonly IDestinationService _destinationService;
    private readonly IRgbMatrixService _matrixService;
    public ScrollDestinationWorker(IDestinationService destinationService, IRgbMatrixService matrixService)
    {
        _destinationService = destinationService;
        _matrixService = matrixService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
         _destinationService.ScrollTextPos = 0;
        while (!stoppingToken.IsCancellationRequested)
        {

            if (_destinationService.ScrollTextPos <= (_matrixService.Canvas.Width -_destinationService.DestinationWidthInPixels))
            {
                await Task.Delay(2000, stoppingToken);
                _destinationService.ScrollTextPos = 0;
                await Task.Delay(5000, stoppingToken);
            }

            if (_destinationService.IsDestinationScrollable)
            {
                _destinationService.ScrollTextPos -= 1;
            }

            await Task.Delay(100, stoppingToken);
        }
    }
}
