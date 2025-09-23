using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using Polly;
namespace LearnRabbitMQ
{
     class Program
    {
        static void Main()
        {
            var retryp = Policy.Handle<Exception>().
                           RetryAsync(10, (ex, att) => {
                               Console.WriteLine(att);
                               Thread.Sleep(1000);
                           });
            retryp.ExecuteAsync(async () =>
            {
                await SendMessage();// code execute
            });
            
            Console.WriteLine("Hello, World!");
            Console.ReadLine();
        }
        async static void ReadMessage()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Queue must already exist (either created in UI or by producer)
            string queueName = "localdelivery";

            Console.WriteLine(" [*] Waiting for messages...");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += Consumer_ReceivedAsync; ;

            // Start consuming messages
            await channel.BasicConsumeAsync(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            // Keep the console app running
            await Task.Delay(Timeout.Infinite);
            Console.ReadLine();
        }

        private async static Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received: {message}");



            Console.WriteLine($" [x] Done processing: {message}");
        }
        static async Task SendMessage()
        {
            // 1. Connect to RabbitMQ (default: localhost:5672)
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("localhello", ExchangeType.Topic, true);

            // 2. Send message directly to the existing "hello" queue
            string message = "Hello RabbitMQ! all";
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "localhello",        // default exchange
                routingKey: "local.hello.evr", // queue name
                body: body);

            Console.WriteLine($" [x] Sent: {message}");
        }
    }
}
