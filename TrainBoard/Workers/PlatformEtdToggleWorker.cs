using TrainBoard.Services;
using Microsoft.Extensions.Caching.Memory;
using TrainBoard.Entities;

namespace TrainBoard.Workers;

public class PlatformEtdToggleWorker : BackgroundService
{
    private readonly IPlatformEtdService _platformEtdService;
    private readonly IMemoryCache _cache;
    public PlatformEtdToggleWorker(IPlatformEtdService platformEtdService, IMemoryCache cache)
    {
        _platformEtdService = platformEtdService;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            _cache.TryGetValue("departureBoard", out ScreenData data);

            if (data != null)
            {
                if ((bool)data.IsCancelled)
                {
                _platformEtdService.ShowPlatform = false; 
                }

                _platformEtdService.ShowPlatform = !_platformEtdService.ShowPlatform;
                await Task.Delay(6000, stoppingToken);
            }

            
        }
    }
}
