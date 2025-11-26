using Xunit;
using BasketMS.RabbitMQ;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.UnitTests.Messaging;

public class SenderIntegrationTests
{
    [Fact]
    public async Task SendMessage_PublishesMessageToQueue()
    {
        // Arrange
        var sender = new Sender();
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // Ensure the queue exists before purging
        await channel.QueueDeclareAsync(
            queue: "stock_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        await channel.QueuePurgeAsync("stock_queue");

        // Act
        await sender.SendMessage(1, 2, 3);

        // Assert
        var result = await channel.BasicGetAsync("stock_queue", true);
        Assert.NotNull(result);

        var body = Encoding.UTF8.GetString(result.Body.ToArray());
        var message = JsonSerializer.Deserialize<JsonElement>(body);

        Assert.Equal(1, message.GetProperty("Id").GetInt32());
        Assert.Equal(2, message.GetProperty("Quantity").GetInt32());
        Assert.Equal(3, message.GetProperty("BasketId").GetInt32());
    }
}
