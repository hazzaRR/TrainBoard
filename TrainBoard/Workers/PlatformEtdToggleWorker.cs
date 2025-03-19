using TrainBoard.Services;

namespace TrainBoard.Workers;

public class PlatformEtdToggleWorker : BackgroundService
{
    private readonly IPlatformEtdService _platformEtdService;
    public PlatformEtdToggleWorker(IPlatformEtdService platformEtdService)
    {
        _platformEtdService = platformEtdService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            _platformEtdService.ShowPlatform = !_platformEtdService.ShowPlatform;
            await Task.Delay(3000, stoppingToken);
        }
    }
}
