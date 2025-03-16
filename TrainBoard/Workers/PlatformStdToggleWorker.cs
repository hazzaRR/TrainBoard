using TrainBoard.Services;

namespace TrainBoard.Workers;

public class PlatformStdToggleWorker : BackgroundService
{
    private readonly IPlatformStdService _platformStdService;
    public PlatformStdToggleWorker(IPlatformStdService platformStdService)
    {
        _platformStdService = platformStdService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            _platformStdService.ShowPlatform = !_platformStdService.ShowPlatform;
            await Task.Delay(8000, stoppingToken);
        }
    }
}
