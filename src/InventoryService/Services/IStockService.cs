using InventoryService.DTOs;

namespace InventoryService.Services;

public interface IStockService
{
    Task<IReadOnlyList<ProductStockResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ProductStockResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ProductStockResponse?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<ProductStockResponse> CreateAsync(ProductStockCreateRequest request, CancellationToken cancellationToken = default);

    Task<ProductStockResponse?> AdjustAsync(int id, ProductStockAdjustRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}