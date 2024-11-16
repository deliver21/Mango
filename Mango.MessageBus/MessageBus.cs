using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    // Mango.MessageBus is a library class project
    public class MessageBus : IMessageBus
    {
        private string connectionString = ""; //Should Get it from Azure Service Bus
        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);
            //Send Message
            ServiceBusSender sender = client.CreateSender(topic_queue_Name);
            string jsonMessage=JsonConvert.SerializeObject(message);

            //Configure the final message
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId=Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
