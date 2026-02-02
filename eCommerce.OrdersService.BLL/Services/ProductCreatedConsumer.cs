using System.Text;
using System.Text.Json;
using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.BLL.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace eCommerce.OrdersService.BLL.Services;

public class ProductCreatedConsumer(
    ILogger<ProductCreatedConsumer> logger,
    IConnection connection) : BackgroundService
{
    private readonly ILogger<ProductCreatedConsumer> _logger = logger;
    private readonly IConnection _connection = connection;
    private IChannel _channel;


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // It is safe to declare the Exchange again here to ensure it exists.
        await _channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.ProductsExchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);

        // Declare the Queue for this specific service
        await _channel.QueueDeclareAsync(
            queue: RabbitMqConstants.OrdersProductCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var keys = new[]
        {
            RabbitMqConstants.ProductsRoutingKeys.ProductCreated,
            RabbitMqConstants.ProductsRoutingKeys.ProductDeleted,
            RabbitMqConstants.ProductsRoutingKeys.ProductStockUpdated,
            RabbitMqConstants.ProductsRoutingKeys.ProductUpdated
        };

        //Bind the Queue with the Exchange using Routing key
        foreach (var key in keys)
        {
            await _channel.QueueBindAsync(
                queue: RabbitMqConstants.OrdersProductCreatedQueue,
                exchange: RabbitMqConstants.ProductsExchange,
                routingKey: key,
                cancellationToken: cancellationToken);
        }


        _logger.LogInformation("Listening for Product Events...");

        //Create the async consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);

        //Handler implementation
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = args.RoutingKey;

                _logger.LogInformation("Received Message: {Message}", message);
                var productDto = JsonSerializer.Deserialize<ProductDTO>(message);

                switch (routingKey)
                {
                    case RabbitMqConstants.ProductsRoutingKeys.ProductCreated:
                        await HandleProductCreated(productDto);
                        break;
                    case RabbitMqConstants.ProductsRoutingKeys.ProductUpdated:
                        await HandleProductUpdated(productDto);
                        break;
                    case RabbitMqConstants.ProductsRoutingKeys.ProductDeleted:
                        await HandleProductDeleted(productDto);
                        break;
                    case RabbitMqConstants.ProductsRoutingKeys.ProductStockUpdated:
                        await HandleStockUpdate(productDto);
                        break;
                    default:
                        _logger.LogWarning("Unknown routing key: {Key}", routingKey);
                        break;
                }

                // 4. Acknowledge (ACK)
                await _channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                // Negative Ack (Nack). Send to Dead Letter 
                await _channel.BasicNackAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: cancellationToken);
            }
        };

        //Start Consuming
        await _channel.BasicConsumeAsync(
            queue: RabbitMqConstants.OrdersProductCreatedQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (TaskCanceledException)
        {
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
    
    // --- Handler Methods ---

    private Task HandleProductCreated(ProductDTO product)
    {
        _logger.LogInformation("Creating local copy of product: {Id}", product.Id);
        // Call your Repository here to Insert
        return Task.CompletedTask;
    }

    private Task HandleProductUpdated(ProductDTO product)
    {
        _logger.LogInformation("Updating local product: {Id}", product.Id);
        // Call your Repository here to Update
        return Task.CompletedTask;
    }
    
    private Task HandleProductDeleted(ProductDTO product)
    {
        _logger.LogInformation("Removing product: {Id}", product.Id);
        // Call your Repository here to Delete (or Soft Delete)
        return Task.CompletedTask;
    }

    private Task HandleStockUpdate(ProductDTO product)
    {
        _logger.LogInformation("Syncing stock for: {Id}. New Qty: {Qty}", product.Id, product.QuantityInStock);
        // Call your Repository here to Update only the Stock field
        return Task.CompletedTask;
    }
}