using InventoryService.DTOs;

namespace InventoryService.Services;

public interface IDonorService
{
    Task<IReadOnlyList<DonorResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DonorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<DonorResponse> CreateAsync(DonorCreateRequest request, CancellationToken cancellationToken = default);

    Task<DonorResponse?> UpdateAsync(int id, DonorUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}