using InventoryService.Data;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repositories;

public sealed class StockRepository : IStockRepository
{
    private readonly InventoryDbContext _dbContext;

    public StockRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductStock>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductStocks.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<ProductStock?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductStocks.FirstOrDefaultAsync(stock => stock.Id == id, cancellationToken);
    }

    public Task<ProductStock?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductStocks.FirstOrDefaultAsync(stock => stock.ProductId == productId, cancellationToken);
    }

    public async Task<ProductStock> CreateAsync(ProductStock stock, CancellationToken cancellationToken = default)
    {
        _dbContext.ProductStocks.Add(stock);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return stock;
    }

    public async Task<ProductStock?> UpdateAsync(ProductStock stock, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.ProductStocks.FirstOrDefaultAsync(item => item.Id == stock.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.DonorId = stock.DonorId;
        existing.ProductId = stock.ProductId;
        existing.QuantityAvailable = stock.QuantityAvailable;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.ProductStocks.FirstOrDefaultAsync(stock => stock.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _dbContext.ProductStocks.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}