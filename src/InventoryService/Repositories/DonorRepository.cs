using InventoryService.Data;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repositories;

public sealed class DonorRepository : IDonorRepository
{
    private readonly InventoryDbContext _dbContext;

    public DonorRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Donor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Donors.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Donor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Donors.FirstOrDefaultAsync(donor => donor.Id == id, cancellationToken);
    }

    public async Task<Donor> CreateAsync(Donor donor, CancellationToken cancellationToken = default)
    {
        _dbContext.Donors.Add(donor);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return donor;
    }

    public async Task<Donor?> UpdateAsync(Donor donor, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Donors.FirstOrDefaultAsync(item => item.Id == donor.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = donor.Name;
        existing.Email = donor.Email;
        existing.Phone = donor.Phone;
        existing.City = donor.City;
        existing.Neighborhood = donor.Neighborhood;
        existing.Street = donor.Street;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Donors.FirstOrDefaultAsync(donor => donor.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _dbContext.Donors.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}