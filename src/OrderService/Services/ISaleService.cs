using OrderService.DTOs;

namespace OrderService.Services;

public interface ISaleService
{
    Task<IReadOnlyList<SaleResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SaleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<SaleResponse> CreateAsync(SaleCreateRequest request, CancellationToken cancellationToken = default);

    Task<SaleResponse?> UpdateAsync(int id, SaleUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsSaleOpenAsync(CancellationToken cancellationToken = default);
}