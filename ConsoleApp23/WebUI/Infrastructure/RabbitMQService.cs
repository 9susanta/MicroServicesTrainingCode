using CustomerManagementMicroService.Domain;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;

namespace UI.Infrastructure
{
    public interface IMessageService<T>
    {
         Task SendMessage(T obj);
    }
    public class RabbitMQService : IMessageService<Customer>
    {
       
        public  async Task SendMessage(Customer obj)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();
                await channel.ExchangeDeclareAsync("CustomerExchange",
                                                    ExchangeType.Direct,
                                                    true);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true // Pretty print
                };

                string json = JsonSerializer.Serialize(obj , options);
                var body = Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync(
                   exchange: "CustomerExchange",        // default exchange
                   routingKey: "CustomerQueue", // queue name
                   body: body);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
