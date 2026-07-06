using ProductCatalogService.Models;

namespace ProductCatalogService.Repositories;

public interface ICategoryRepository
{
    Task<CategoryDocument> CreateAsync(CategoryDocument category, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryDocument>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CategoryDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<CategoryDocument?> GetByLegacyIdAsync(int legacyId, CancellationToken cancellationToken = default);

    Task<CategoryDocument?> UpdateAsync(CategoryDocument category, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}