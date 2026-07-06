namespace Shared.Contracts;

public sealed record InventoryReservedEvent(
    Guid CorrelationId,
    int OrderId,
    int ProductId,
    int ReservedQuantity,
    DateTime ReservedAtUtc);
