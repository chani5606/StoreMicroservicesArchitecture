using ProductCatalogService.Models;

namespace ProductCatalogService.Repositories;

public interface IProductRepository
{
    Task<ProductDocument> CreateAsync(ProductDocument product, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductDocument>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ProductDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<ProductDocument?> GetByLegacyIdAsync(int legacyId, CancellationToken cancellationToken = default);

    Task<ProductDocument?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<ProductDocument?> UpdateAsync(ProductDocument product, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}