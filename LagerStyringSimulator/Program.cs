using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace StockControleSimulator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            DbSimulator db = new DbSimulator();

            //Create the connection factory
            var factory = new ConnectionFactory() { HostName = "localhost" };

            //Establish the connection and channel
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            //Declare the queue
            await channel.QueueDeclareAsync(queue: "stock_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine("Waiting for message");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                
                try
                {
                    var recievedDto = JsonConvert.DeserializeObject<RecieverDTO>(json);
                    Console.WriteLine(json);
                    db.ReserveItem(recievedDto.Id, recievedDto.Quantity, recievedDto.BasketId);
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }


                catch (Exception ex)
                {
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync("stock_queue", autoAck: false, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }
        private record RecieverDTO(int Id, int Quantity, string BasketId);

    }
}
