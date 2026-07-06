namespace OrderService.DTOs;

public sealed record BasketCreateRequest(int LegacyId, int UserId, int ProductId, int Quantity, decimal UnitPrice);

public sealed record BasketUpdateStatusRequest(string Status);

public sealed record BasketResponse(
    int Id,
    Guid CorrelationId,
    int LegacyId,
    int UserId,
    int ProductId,
    int Quantity,
    string Status,
    decimal UnitPrice,
    DateTime CreatedAtUtc);