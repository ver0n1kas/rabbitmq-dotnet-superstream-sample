namespace RabbitMQ.Client.StreamTest;

using System.Net;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Stream.Client;

public class RabbitStreamClient {
  
    static async Task Main(string[] args) {

        List<Task> tasks = new();

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                 var ep = new IPEndPoint(IPAddress.Parse(options.RabbitHost), 5552);
                var resolver = new AddressResolver(ep);
                var config = new StreamSystemConfig()
                {
                    AddressResolver = resolver,
                    UserName = options.Username,
                    Password = options.Password,
                    Endpoints = new List<EndPoint>() {resolver.EndPoint},
                    ConnectionPoolConfig = new ConnectionPoolConfig()
                    {
                        ProducersPerConnection = options.ProducersPerConnection,
                        ConsumersPerConnection = options.ConsumersPerConnection,
                    }
                };

                for (int i = 0; i < options.Consumers; i++) {
                    tasks.Add(Task.Run(() => SuperStreamConsumer.RunConsumerInstance("consumer-" + i, options.SuperStreamName, config)));
                }

                for (int i = 0; i < options.Consumers; i++) {
                    tasks.Add(Task.Run(() => SuperStreamPublisher.RunPublisher(options.Message + i, options.SuperStreamName, config)));
                }
            });

            await Task.WhenAll(tasks);
    }
}