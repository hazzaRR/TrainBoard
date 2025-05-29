using Microsoft.AspNetCore.Components;
using TrainBoardDashboard.Entities;
using MQTTnet;
using System.Text.Json;
using TrainBoardDashboard.Services;

namespace TrainBoardDashboard;

public partial class Home: IAsyncDisposable
{
    [Inject]
    private NavigationManager Navigation { get; set; }

    [Inject]
    private ILogger<Home> Logger { get; set; }

    [Inject]
    private MqttService MqttService {get; set;}
    private int NumRows { get; set; } = 1;
    private string Crs { get; set; } = "";
    private string FilterCrs { get; set; } = "";
    private int TimeOffset { get; set; } = 0;
    private int TimeWindow { get; set; } = 0;

    protected override async Task OnInitializedAsync()
    {
        
        MqttService.OnMessageReceived += UpdateConfiguration;
        UpdateConfiguration(MqttService.CurrentConfig);

        await base.OnInitializedAsync();
    }

    protected async Task UpdateMatrixConfig()
    {
        var newConfiguration = new RgbMatrixConfiguration()
        {
            NumRows = NumRows,
            Crs = Crs,
            FilterCrs = FilterCrs,
            TimeOffset = TimeOffset,
            TimeWindow = TimeWindow
        };


        var payload = JsonSerializer.Serialize(newConfiguration);
        await MqttService.PublishAsync("matrix_config", payload);
    }

    protected void UpdateConfiguration(RgbMatrixConfiguration config)
    {
            NumRows = config.NumRows;
            Crs = config.Crs;
            FilterCrs = config.FilterCrs;
            TimeOffset = config.TimeOffset;
            TimeWindow = config.TimeWindow;

            InvokeAsync(StateHasChanged);
    }

    public async ValueTask DisposeAsync()
    {
    if (MqttService != null)
    {
        MqttService.OnMessageReceived -= UpdateConfiguration;
    }

    GC.SuppressFinalize(this);
    await Task.CompletedTask;
    }
}