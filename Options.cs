using CommandLine;

public class Options {
    [Option('r', "rabbithost", Required = false, HelpText = "Hostname or IP of RabbitMQ instance or loadbalancer", Default = "localhost")]
    public string RabbitHost { get; set; }

    [Option('u', "username", Required = false, HelpText = "RabbitMQ user", Default = "guest")]
    public string Username { get; set; }

    [Option('p', "password", Required = false, HelpText = "RabbitMQ password", Default = "guest")]
    public string Password { get; set; }

    [Option('s', "super_stream_name", Required = false, HelpText = "The name of the superstream", Default = "superstream")]
    public string SuperStreamName { get; set; }

    [Option('m', "message", Required = false, HelpText = "Message to publish", Default = "test")]
    public string Message { get; set; }

    [Option('c', "consumers", Required = false, HelpText = "Number of consumers", Default = 1)]
    public int Consumers { get; set; }

    [Option('d', "producers", Required = false, HelpText = "Number of producers", Default = 1)]
    public int Producers { get; set; }

    [Option('b', "consumers_per_connection", Required = false, HelpText = "Number of consumers per connection", Default = 7)]
    public byte ConsumersPerConnection { get; set; }

    [Option('y', "producers", Required = false, HelpText = "Number of producers per connection", Default =  (byte) 8)]
    public byte ProducersPerConnection { get; set; }


}