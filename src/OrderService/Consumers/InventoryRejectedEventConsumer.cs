using MassTransit;
using OrderService.Data;
using Shared.Contracts;

namespace OrderService.Consumers;

public sealed class InventoryRejectedConsumer : IConsumer<InventoryRejectedEvent>
{
    private readonly OrderDbContext _dbContext;
    private readonly ILogger<InventoryRejectedConsumer> _logger;

    public InventoryRejectedConsumer(OrderDbContext dbContext, ILogger<InventoryRejectedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryRejectedEvent> context)
    {
        var message = context.Message;
        var basket = await _dbContext.Baskets.FindAsync([message.OrderId], context.CancellationToken);
        if (basket is null)
        {
            _logger.LogWarning("Order {OrderId} not found for rejection event {CorrelationId}", message.OrderId, message.CorrelationId);
            return;
        }

        basket.Status = "Cancelled";
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Order {OrderId} cancelled due to inventory rejection ({Reason}). CorrelationId: {CorrelationId}", message.OrderId, message.Reason, message.CorrelationId);
    }
}
