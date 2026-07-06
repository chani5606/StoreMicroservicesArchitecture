using InventoryService.Models;

namespace InventoryService.Repositories;

public interface IStockRepository
{
    Task<IReadOnlyList<ProductStock>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ProductStock?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ProductStock?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<ProductStock> CreateAsync(ProductStock stock, CancellationToken cancellationToken = default);

    Task<ProductStock?> UpdateAsync(ProductStock stock, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}