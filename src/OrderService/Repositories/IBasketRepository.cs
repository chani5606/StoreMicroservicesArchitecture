using OrderService.Models;

namespace OrderService.Repositories;

public interface IBasketRepository
{
    Task<IReadOnlyList<Basket>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Basket?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Basket> CreateAsync(Basket basket, CancellationToken cancellationToken = default);

    Task<Basket?> UpdateAsync(Basket basket, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}