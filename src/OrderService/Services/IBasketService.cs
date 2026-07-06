using OrderService.DTOs;

namespace OrderService.Services;

public interface IBasketService
{
    Task<IReadOnlyList<BasketResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BasketResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BasketResponse> CreateAsync(BasketCreateRequest request, CancellationToken cancellationToken = default);

    Task<BasketResponse?> UpdateStatusAsync(int id, BasketUpdateStatusRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}