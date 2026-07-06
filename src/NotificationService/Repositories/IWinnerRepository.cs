using NotificationService.Models;

namespace NotificationService.Repositories;

public interface IWinnerRepository
{
    Task<IReadOnlyList<Winner>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Winner?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Winner> CreateAsync(Winner winner, CancellationToken cancellationToken = default);
}