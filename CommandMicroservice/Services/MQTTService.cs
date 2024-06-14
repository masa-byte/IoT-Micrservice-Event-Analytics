using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CommandMicroservice.Services
{
    public class MQTTService : BackgroundService
    {
        private readonly IMqttClient mqttClient;
        private readonly MqttClientOptions mqttOptions;
        private readonly IHubContext<EventHub> hubContext;
        private static int messageId = 0;

        public MQTTService(IHubContext<EventHub> hubContext)
        {
            this.hubContext = hubContext;

            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId("CommandMicroservice")
                .WithTcpServer("localhost", 8883)
                .WithCleanSession()
                .Build();


            mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Connected successfully with MQTT broker.");

                // Subscribing to the topic
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic("ekuiper/data")
                    .Build());

                Console.WriteLine("Subscribed to topic");
            };

            mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("Disconnected from MQTT broker.");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Reconnect failed: {ex.Message}");
                }
            };

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                Console.WriteLine($"Received message: {message}");

                EventInfo eventInfo = new EventInfo();

                var messageDict = new Dictionary<string, object>();

                using (JsonDocument doc = JsonDocument.Parse(message))
                {
                    foreach (JsonProperty element in doc.RootElement.EnumerateObject())
                    {
                        messageDict[element.Name] = element.Value.ToString();
                    }
                }

                eventInfo.Id = messageId + 1;
                messageId++;

                if (messageDict.ContainsKey("AvgTemperature_C"))
                {
                    double value = double.Parse(messageDict["AvgTemperature_C"].ToString()!);
                    if (value > 24)
                    {
                        eventInfo.EventType = "High Temperature Alarm";
                    }
                    else if (value < 18)
                    {
                        eventInfo.EventType = "Low Temperature Alarm";
                    }
                    eventInfo.Value = value;
                }
                else if (messageDict.ContainsKey("MaxpH"))
                {
                    double value = double.Parse(messageDict["MaxpH"].ToString()!);
                    if (value > 8)
                    {
                        eventInfo.EventType = "High pH Alarm";
                    }
                    else if (value < 5)
                    {
                        eventInfo.EventType = "Low pH Alarm";
                    }
                    eventInfo.Value = value;
                }

                eventInfo.LocationId = messageDict["PondId"].ToString()!;
                eventInfo.EventDate = DateTime.Now;

                await hubContext.Clients.All.SendAsync("ReceiveEvent", eventInfo);

                await Task.CompletedTask;
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!mqttClient.IsConnected)
                {
                    try
                    {
                        await mqttClient.ConnectAsync(mqttOptions, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"MQTT connection failed: {ex.Message}");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Retrying every 5 seconds if not connected
            }
        }
    }
}
