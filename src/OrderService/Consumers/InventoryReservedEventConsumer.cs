using MassTransit;
using OrderService.Data;
using Shared.Contracts;

namespace OrderService.Consumers;

public sealed class InventoryReservedConsumer : IConsumer<InventoryReservedEvent>
{
    private readonly OrderDbContext _dbContext;
    private readonly ILogger<InventoryReservedConsumer> _logger;

    public InventoryReservedConsumer(OrderDbContext dbContext, ILogger<InventoryReservedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
    {
        var message = context.Message;
        var basket = await _dbContext.Baskets.FindAsync([message.OrderId], context.CancellationToken);
        if (basket is null)
        {
            _logger.LogWarning("Order {OrderId} not found for reservation event {CorrelationId}", message.OrderId, message.CorrelationId);
            return;
        }

        basket.Status = "Confirmed";
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Order {OrderId} confirmed via inventory reservation. CorrelationId: {CorrelationId}", message.OrderId, message.CorrelationId);
    }
}
