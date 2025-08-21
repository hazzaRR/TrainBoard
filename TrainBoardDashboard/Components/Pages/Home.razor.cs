using Microsoft.AspNetCore.Components;
using TrainBoardDashboard.Entities;
using MQTTnet;
using System.Text.Json;
using TrainBoardDashboard.Services;

namespace TrainBoardDashboard;

public partial class Home : IAsyncDisposable
{
    [Inject]
    private NavigationManager Navigation { get; set; }

    [Inject]
    private ILogger<Home> Logger { get; set; }

    [Inject]
    private MqttService MqttService { get; set; }
    [Inject]
    private StationService StationService { get; set; }
    private int NumRows { get; set; } = 1;
    private string Crs { get; set; } = "COL";
    private string FilterCrs { get; set; } = "";
    private string FilterType { get; set; } = "to";
    private int TimeOffset { get; set; } = 2;
    private int TimeWindow { get; set; } = 120;
    private string StdColour { get; set; } = "#ffa000";
    private string PlatformColour { get; set; } = "#ffa000";
    private string DestinationColour { get; set; } = "#ffa000";
    private string CallingPointsColour { get; set; } = "#ffa000";
    private string CurrentTimeColour { get; set; } = "#ffa000";
    private string DelayColour { get; set; } = "#ff0f00";
    private string OnTimeColour { get; set; } = "#00ff00";
    private bool ShowAlert { get; set; } = false;
    private bool ShowCustomDisplay { get; set; } = false;
    public string[][] MatrixPixels { get; set; } = new string[32][];
    private List<Station> Stations { get; set; } = [];


    protected override async Task OnInitializedAsync()
    {
        InitialiseArray();
        MqttService.OnMessageReceived += UpdateConfiguration;
        UpdateConfiguration(MqttService.CurrentConfig);
        Stations = await StationService.GetStationsAsync();


        await base.OnInitializedAsync();
    }

    protected async Task ResetMatrixConfig()
    {
        NumRows = 1;
        Crs = "COL";
        FilterCrs = "";
        FilterType = "to";
        TimeOffset = 2;
        TimeWindow = 120;
        StdColour = "#ffa000";
        PlatformColour = "#ffa000";
        DestinationColour = "#ffa000";
        CallingPointsColour = "#ffa000";
        CurrentTimeColour = "#ffa000";
        DelayColour = "#ff0f00";
        OnTimeColour = "#00ff00";
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
            TimeWindow = TimeWindow,
            StdColour = StdColour,
            DestinationColour = DestinationColour,
            PlatformColour = PlatformColour,
            CallingPointsColour = CallingPointsColour,
            CurrentTimeColour = CurrentTimeColour,
            DelayColour = DelayColour,
            OnTimeColour = OnTimeColour,
            ShowCustomDisplay = ShowCustomDisplay,
            MatrixPixels = MatrixPixels,
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
        StdColour = config.StdColour;
        DestinationColour = config.DestinationColour;
        PlatformColour = config.PlatformColour;
        CallingPointsColour = config.CallingPointsColour;
        CurrentTimeColour = config.CurrentTimeColour;
        DelayColour = config.DelayColour;
        OnTimeColour = config.OnTimeColour;
        ShowCustomDisplay = config.ShowCustomDisplay;
        MatrixPixels = config.MatrixPixels;
        InitialiseArray();
        InvokeAsync(StateHasChanged);
    }

    protected void InitialiseArray()
    {
        if (MatrixPixels == null || MatrixPixels.Length == 0 )
        {
            MatrixPixels = new string[32][]; 
        }

        for (int i = 0; i < MatrixPixels.Length; i++)
            {
                if (MatrixPixels[i] == null || MatrixPixels[i].Length == 0)
                {
                    MatrixPixels[i] = new string[64];
                    for (int j = 0; j < MatrixPixels[i].Length; j++)
                    {
                        MatrixPixels[i][j] = "#000000";
                    }
                }
            }
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