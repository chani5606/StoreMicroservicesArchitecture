using MassTransit;
using Shared.Contracts;

namespace NotificationService.Consumers;

public sealed class OrderStatusConsumer :
    IConsumer<InventoryReservedEvent>,
    IConsumer<InventoryRejectedEvent>
{
    private readonly ILogger<OrderStatusConsumer> _logger;

    public OrderStatusConsumer(ILogger<OrderStatusConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<InventoryReservedEvent> context)
    {
        _logger.LogInformation(
            "Notification sent to customer: Order status updated! CorrelationId: {CorrelationId}, OrderId: {OrderId}, Status: Confirmed",
            context.Message.CorrelationId,
            context.Message.OrderId);

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<InventoryRejectedEvent> context)
    {
        _logger.LogInformation(
            "Notification sent to customer: Order status updated! CorrelationId: {CorrelationId}, OrderId: {OrderId}, Status: Cancelled",
            context.Message.CorrelationId,
            context.Message.OrderId);

        return Task.CompletedTask;
    }
}
