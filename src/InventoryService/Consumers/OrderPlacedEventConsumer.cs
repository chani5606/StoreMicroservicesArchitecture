using InventoryService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace InventoryService.Consumers;

public sealed class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly InventoryDbContext _dbContext;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(
        InventoryDbContext dbContext,
        ILogger<OrderPlacedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var message = context.Message;
        var stock = await _dbContext.ProductStocks
            .FirstOrDefaultAsync(s => s.ProductId == message.ProductId, context.CancellationToken);

        if (stock is not null && stock.QuantityAvailable >= message.Inventory)
        {
            stock.QuantityAvailable -= message.Inventory;
            stock.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await context.Publish(
                new InventoryReservedEvent(
                    message.CorrelationId,
                    message.OrderId,
                    message.ProductId,
                    message.Inventory,
                    DateTime.UtcNow),
                context.CancellationToken);

            _logger.LogInformation("Inventory reserved for order {OrderId}. CorrelationId: {CorrelationId}", message.OrderId, message.CorrelationId);
            return;
        }

        await context.Publish(
            new InventoryRejectedEvent(
                message.CorrelationId,
                message.OrderId,
                message.ProductId,
                message.Inventory,
                "Insufficient stock",
                DateTime.UtcNow),
            context.CancellationToken);

        _logger.LogInformation("Inventory rejected for order {OrderId}. CorrelationId: {CorrelationId}", message.OrderId, message.CorrelationId);
    }
}
