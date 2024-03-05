using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.AMQP;
using RabbitMQ.Stream.Client.Reliable;

namespace RabbitMQ.Client.StreamTest;
public class SuperStreamPublisher {


    public static async Task RunPublisher(string additionalMessage, string superStreamName, StreamSystemConfig config)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole();

            builder.AddFilter("RabbitMQ.Stream", LogLevel.Information);
        });

        var logger = loggerFactory.CreateLogger<Producer>();

        var loggerMain = loggerFactory.CreateLogger<RabbitStreamClient>();

        loggerMain.LogInformation("Starting SuperStream Producer");


        try {
            var system = await StreamSystem.Create(config).ConfigureAwait(false);
                  loggerMain.LogInformation("Super Stream Producer connected to RabbitMQ");
            //TODO: Creation of superstreams is only supported in RMQ version 3.13 and up
            // in the meantime exec into one of the pods and run `rabbitmq-streams add_super_stream invoices --partitions 3`
            // await system.CreateSuperStream(new PartitionsSuperStreamSpec(Constants.SUPERSTREAM_NAME, 3)).ConfigureAwait(false);
            var producer = await Producer.Create(
                new ProducerConfig(system,
                        superStreamName) // <1>
                    {
                        SuperStreamConfig = new SuperStreamConfig() // <2>
                        {
                            Routing = msg => msg.Properties.MessageId.ToString() // <3>
                        }
                    }, logger).ConfigureAwait(false);
            int i = 1;
            while (true)
            {
                var message = new Message(Encoding.Default.GetBytes($"my_invoice_number{i}. {additionalMessage}")) // <4>
                
                {
                    Properties = new Properties() {MessageId = $"id_{i}"}
                };
                await producer.Send(message).ConfigureAwait(false);
                loggerMain.LogInformation("Sent {I} message to {StreamName}, id: {ID}", $"my_invoice_number{i}",
                    superStreamName, $"id_{i}");
                Thread.Sleep(TimeSpan.FromMilliseconds(5000));
                i++;
            }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
    }
}
