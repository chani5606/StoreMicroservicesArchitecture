namespace Shared.Contracts;

public sealed record OrderPlacedEvent(
    Guid CorrelationId,
    int OrderId,
    int ProductId,
    int Inventory,
    DateTime PlacedAtUtc);
