using System.Dynamic;
using System.Runtime.CompilerServices;

namespace RabbitMQ.Client.StreamTest;

public class RmqConfig
{
   private string rmqHost;
   private string username;
   private string password;

   private string superstreamName;

   private byte producersPerConnection;
   private byte consumersPerConnection;

   public RmqConfig(string rmqHost, 
      string username, 
      string password, 
      string superstreamName, 
      byte producersPerConnection, 
      byte consumersPerConnection) {
      
      this.rmqHost = rmqHost;
      this.username = username;
      this.password = password;
      this.superstreamName = superstreamName;
      this.producersPerConnection = producersPerConnection;
      this.consumersPerConnection = consumersPerConnection;
   }

   public string getRmqHost() {
      return this.rmqHost;
   }
   public string getUsername() {
      return this.username;
   }
   public string getPassword() {
      return this.password;
   }
   public string getSuperstreamName() {
      return this.superstreamName;
   }

   public byte getProducersPerConnection() {
      return this.producersPerConnection;
   }

   public byte getConsumersPerConnection() {
      return this.consumersPerConnection;
   }
}
