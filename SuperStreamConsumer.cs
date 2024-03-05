using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.AMQP;
using RabbitMQ.Stream.Client.Reliable;

namespace RabbitMQ.Client.StreamTest;
public class SuperStreamConsumer {


    public static async Task RunConsumerInstance(string consumerName, string superStreamName, StreamSystemConfig config)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole();
            builder.AddFilter("RabbitMQ.Stream", LogLevel.Information);
        });

        var logger = loggerFactory.CreateLogger<Consumer>();
        var loggerMain = loggerFactory.CreateLogger<SuperStreamConsumer>();
        loggerMain.LogInformation("Starting SuperStream Consumer {ConsumerName}", consumerName);

        var system = await StreamSystem.Create(config).ConfigureAwait(false);

        Console.WriteLine("Super Stream Consumer connected to RabbitMQ. ConsumerName {0}", consumerName);
        var consumer = await Consumer.Create(new ConsumerConfig(system, superStreamName)
        {
            IsSuperStream = true, 
            Reference = "MyApp",
            OffsetSpec = new OffsetTypeFirst(),
            MessageHandler = async (stream, consumerSource, context, message) => 
            {
                loggerMain.LogInformation("Consumer Name {ConsumerName} " +
                                          "-Received message id: {PropertiesMessageId} body: {S}, Stream {Stream}, Offset {Offset}",
                    consumerName, message.Properties.MessageId, Encoding.UTF8.GetString(message.Data.Contents),
                    stream, context.Offset);
                await consumerSource.StoreOffset(context.Offset).ConfigureAwait(false);
                await Task.CompletedTask.ConfigureAwait(false);
            },
            IsSingleActiveConsumer = true, 
            ConsumerUpdateListener = async (reference, stream, isActive) => // <3>
            {
                loggerMain.LogInformation($"******************************************************");
                loggerMain.LogInformation("reference {Reference} stream {Stream} is active: {IsActive}", reference,
                    stream, isActive);

                ulong offset = 0;
                try
                {
                    offset = await system.QueryOffset(reference, stream).ConfigureAwait(false);
                }
                catch (OffsetNotFoundException e)
                {
                    loggerMain.LogInformation("OffsetNotFoundException {Message}, will use OffsetTypeNext", e.Message);
                    return new OffsetTypeNext();
                }

                if (isActive)
                {
                    loggerMain.LogInformation("Restart Offset {Offset}", offset);
                }

                loggerMain.LogInformation($"******************************************************");
                await Task.CompletedTask.ConfigureAwait(false);
                return new OffsetTypeOffset(offset + 1); // <4>
            },
        }, logger).ConfigureAwait(false);
    }
}
