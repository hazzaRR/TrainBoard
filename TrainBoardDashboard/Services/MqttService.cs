using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using TrainBoardDashboard.Entities;

namespace TrainBoardDashboard.Services;

public class MqttService : BackgroundService
{
    private readonly ILogger<MqttService> _logger;
    private IManagedMqttClient _mqttClient;
    private ManagedMqttClientOptions _options;
    public RgbMatrixConfiguration CurrentConfig {get; private set;} = new();

    public event Action<RgbMatrixConfiguration> OnMessageReceived;

    public MqttService(ILogger<MqttService> logger)
    {
        _logger = logger;

        _mqttClient = new MqttFactory().CreateManagedMqttClient();

        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithClientId("matrixBlazor")
            .WithCleanSession(false) // Persistent session
            .Build();

        // Configure managed MQTT client options (for auto-reconnect)
        _options = new ManagedMqttClientOptionsBuilder()
           .WithClientOptions(clientOptions)
           .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
           .Build();

        SetupMqttEventHandlers();
    }

    public async Task PublishAsync(string topic, string payload)
    {
        if (!_mqttClient.IsConnected)
        {
            _logger.LogWarning("MQTT client is not connected. Message will be enqueued for later publishing.");
        }

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithRetainFlag()
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _mqttClient.EnqueueAsync(message);
        _logger.LogInformation($"Message enqueued to topic '{topic}'.");
    }

    private void SetupMqttEventHandlers()
    {
        _mqttClient.ConnectedAsync += async e =>
        {
            _logger.LogInformation("Connected to MQTT broker.");
            await _mqttClient.SubscribeAsync("matrix_config");
            _logger.LogInformation("Subscribed to matrix_config topic.");
        };

        _mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogWarning($"Disconnected from MQTT broker. Reason: {e.Reason}. Attempting to reconnect...");
            await Task.CompletedTask;
        };

        _mqttClient.ConnectingFailedAsync += async e =>
        {
            _logger.LogError(e.Exception, "MQTT client connection failed.");
            await Task.CompletedTask;
        };

        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            _logger.LogInformation($"Message received on topic '{e.ApplicationMessage.Topic}'.");
            if (e.ApplicationMessage.Topic.Equals("matrix_config"))
            {
                try
                {
                    CurrentConfig = JsonSerializer.Deserialize<RgbMatrixConfiguration>(e.ApplicationMessage.ConvertPayloadToString());

                    OnMessageReceived?.Invoke(CurrentConfig);
                    _logger.LogInformation("Configuration Received and processed.");
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to deserialize RgbMatrixConfiguration from MQTT message.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing MQTT message.");
                }
            }
            await Task.CompletedTask;
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
    {
        _logger.LogInformation("Mqtt Background Service starting..");
        await _mqttClient.StartAsync(_options);
        _logger.LogInformation("Managed MQTT client started and attempting connection.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        _logger.LogInformation("Mqtt Background Service stopping.");

        if (_mqttClient != null)
        {
            await _mqttClient.StopAsync();
        }

    }

    public override async Task StopAsync(CancellationToken stoppingToken = default)
    {
        _logger.LogWarning("Mqtt background service is stopping");

        await base.StopAsync(stoppingToken);
    }
}