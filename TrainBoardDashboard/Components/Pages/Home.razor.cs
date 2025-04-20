using Microsoft.AspNetCore.Components;
using TrainBoardDashboard.Entities;
using MQTTnet;
using System.Text.Json;

namespace TrainBoardDashboard;

public partial class Home: IAsyncDisposable
{


    [Inject]
    private NavigationManager Navigation { get; set; }
    private IMqttClient _mqttClient;
    private MqttClientOptions _options;
    private int NumRows {get; set;} = 1;
    private string Crs {get; set;} = "";
    private string FilterCrs {get; set;} = "";
    private int TimeOffset {get; set;} = 0;
    private int TimeWindow {get; set;} = 0;


    protected override async Task OnInitializedAsync()
    {

        _mqttClient = new MqttClientFactory().CreateMqttClient();

        _options = new MqttClientOptionsBuilder()
        .WithTcpServer("localhost", 1883)
        .WithClientId("matrixBlazor")
        .WithCleanSession()
        .Build();

        SetupMqttEventHandlers();
        
        try 
        {
            await _mqttClient.ConnectAsync(_options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection failed: {ex.Message}");
        }

        await base.OnInitializedAsync();
    }

    protected async Task UpdateMatrixConfig()
    {
        Console.WriteLine("This button was pressed");

        if(_mqttClient.IsConnected)
        {
            var newConfiguration = new RgbMatrixConfiguration()
            {
                NumRows = this.NumRows,
                Crs = this.Crs,
                FilterCrs = this.FilterCrs,
                TimeOffset = this.TimeOffset,
                TimeWindow = this.TimeWindow
            };

            var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("matrix_config")
            .WithPayload(JsonSerializer.Serialize(newConfiguration))
            .WithRetainFlag()
            .Build();

            await _mqttClient.PublishAsync(applicationMessage);
        }
        else
        {
            throw new Exception("Not connected to Mqtt Broker");
        }
    }

    public async ValueTask DisposeAsync()
    {
        
        GC.SuppressFinalize(this);
    }

    private void SetupMqttEventHandlers()
    {

        _mqttClient.ConnectedAsync += async e => 
        {
            await _mqttClient.SubscribeAsync("matrix_config");
        };

        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            if (e.ApplicationMessage.Topic.Equals("matrix_config"))
            {
                RgbMatrixConfiguration config = JsonSerializer.Deserialize<RgbMatrixConfiguration>(e.ApplicationMessage.ConvertPayloadToString());

                NumRows = config.NumRows;
                Crs = config.Crs;
                FilterCrs = config.FilterCrs;
                TimeOffset = config.TimeOffset;
                TimeWindow = config.TimeWindow;
                InvokeAsync(StateHasChanged);
            }
        };

        _mqttClient.DisconnectedAsync += async e => 
        {
            await Task.Delay(5000);

            try
            {
                await _mqttClient.ConnectAsync(_options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reconnection failed: {ex.Message}");
            }
        };

    }


}