using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService.Repositories;

public sealed class WinnerRepository : IWinnerRepository
{
    private readonly NotificationDbContext _dbContext;

    public WinnerRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Winner>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Winners.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Winner?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Winners.FirstOrDefaultAsync(winner => winner.Id == id, cancellationToken);
    }

    public async Task<Winner> CreateAsync(Winner winner, CancellationToken cancellationToken = default)
    {
        _dbContext.Winners.Add(winner);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return winner;
    }
}