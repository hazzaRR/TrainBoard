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
    [Inject]
    private StationService StationService {get; set;}
    private int NumRows { get; set; } = 1;
    private string Crs { get; set; } = "";
    private string FilterCrs { get; set; } = "";
    private string FilterType { get; set; } = "to";
    private int TimeOffset { get; set; } = 0;
    private int TimeWindow { get; set; } = 0;
    private bool ShowAlert { get; set; } = false;
    private List<Station> Stations { get; set; } = [];


    protected override async Task OnInitializedAsync()
    {

        MqttService.OnMessageReceived += UpdateConfiguration;
        UpdateConfiguration(MqttService.CurrentConfig);
        Stations = await StationService.GetStationsAsync();

        await base.OnInitializedAsync();
    }

    protected async Task UpdateMatrixConfig()
    {
        var newConfiguration = new RgbMatrixConfiguration()
        {
            NumRows = NumRows,
            Crs = Crs.ToUpper(),
            FilterCrs = FilterCrs.ToUpper(),
            FilterType = FilterType,
            TimeOffset = TimeOffset,
            TimeWindow = TimeWindow
        };


        var payload = JsonSerializer.Serialize(newConfiguration);
        await MqttService.PublishAsync("matrix_config", payload);

        ShowAlert = true;
        await Task.Delay(10000);
        ShowAlert = false;
    }
    
    protected void UpdateConfiguration(RgbMatrixConfiguration config)
    {
        NumRows = config.NumRows;
        Crs = config.Crs;
        FilterCrs = config.FilterCrs;
        FilterType = config.FilterType;
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