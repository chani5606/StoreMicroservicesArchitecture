using ProductCatalogService.DTOs;

namespace ProductCatalogService.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CategoryResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<CategoryResponse> CreateAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default);

    Task<CategoryResponse?> UpdateAsync(string id, CategoryUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}