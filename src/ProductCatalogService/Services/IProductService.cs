using ProductCatalogService.DTOs;

namespace ProductCatalogService.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ProductResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<ProductResponse?> GetByLegacyIdAsync(int legacyId, CancellationToken cancellationToken = default);

    Task<ProductResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default);

    Task<ProductResponse?> UpdateAsync(string id, ProductUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}