using NotificationService.DTOs;

namespace NotificationService.Services;

public interface IWinnerService
{
    Task<IReadOnlyList<WinnerResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<WinnerResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<WinnerResponse> CreateAsync(WinnerCreateRequest request, CancellationToken cancellationToken = default);
}