using TrainBoardDashboard.Entities;
using System.Text.Json;

namespace TrainBoardDashboard.Services;

public class StationService
{
    private readonly IWebHostEnvironment _env;

    public StationService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<List<Station>> GetStationsAsync()
    {
        var path = Path.Combine(_env.WebRootPath, "data", "national-rail-stations.json");
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<Station>>(json);
    }
}