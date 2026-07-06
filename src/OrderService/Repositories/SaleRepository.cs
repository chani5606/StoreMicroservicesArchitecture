using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly OrderDbContext _dbContext;

    public SaleRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sales.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Sales.FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _dbContext.Sales.Add(sale);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Sales.FirstOrDefaultAsync(item => item.Id == sale.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = sale.Name;
        existing.StartDateUtc = sale.StartDateUtc;
        existing.EndDateUtc = sale.EndDateUtc;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Sales.FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _dbContext.Sales.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IsSaleOpenAsync(DateTime utcNow, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sales.AnyAsync(
            sale => sale.StartDateUtc <= utcNow && sale.EndDateUtc >= utcNow,
            cancellationToken);
    }
}