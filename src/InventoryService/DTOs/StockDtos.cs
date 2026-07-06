namespace InventoryService.DTOs;

public sealed record ProductStockCreateRequest(int ProductId, int DonorId, int QuantityAvailable);

public sealed record ProductStockAdjustRequest(int QuantityDelta);

public sealed record ProductStockResponse(
    int Id,
    int ProductId,
    int DonorId,
    int QuantityAvailable,
    DateTime UpdatedAtUtc);