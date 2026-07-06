namespace Shared.Contracts;

public sealed record InventoryRejectedEvent(
    Guid CorrelationId,
    int OrderId,
    int ProductId,
    int RequestedQuantity,
    string Reason,
    DateTime RejectedAtUtc);
