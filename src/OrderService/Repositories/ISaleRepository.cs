using OrderService.Models;

namespace OrderService.Repositories;

public interface ISaleRepository
{
    Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<Sale?> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsSaleOpenAsync(DateTime utcNow, CancellationToken cancellationToken = default);
}