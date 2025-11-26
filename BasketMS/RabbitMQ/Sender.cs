using RabbitMQ.Client;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace BasketMS.RabbitMQ
{
    public class Sender
    {
        public async Task SendMessage(int id, int quantity, int basketId)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "stock_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var sendDto = new {Id = id, Quantity = quantity, BasketId = basketId};
            var json = JsonSerializer.Serialize(sendDto);

            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true,
            };

            await channel.BasicPublishAsync(exchange: string.Empty, 
                routingKey: "stock_queue", 
                mandatory: true, 
                basicProperties: properties, 
                body: body);

            Console.WriteLine($"Sent message: {json}");
        }
        record SendDto (int Id, int Quantity, int BasketId);
    }
}
