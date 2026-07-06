using InventoryService.Models;

namespace InventoryService.Repositories;

public interface IDonorRepository
{
    Task<IReadOnlyList<Donor>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Donor?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Donor> CreateAsync(Donor donor, CancellationToken cancellationToken = default);

    Task<Donor?> UpdateAsync(Donor donor, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}