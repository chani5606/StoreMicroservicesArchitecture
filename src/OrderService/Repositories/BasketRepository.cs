using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Repositories;

public sealed class BasketRepository : IBasketRepository
{
    private readonly OrderDbContext _dbContext;

    public BasketRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Basket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Baskets.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Basket?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Baskets.FirstOrDefaultAsync(basket => basket.Id == id, cancellationToken);
    }

    public async Task<Basket> CreateAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        _dbContext.Baskets.Add(basket);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return basket;
    }

    public async Task<Basket?> UpdateAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Baskets.FirstOrDefaultAsync(item => item.Id == basket.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Status = basket.Status;
        existing.Quantity = basket.Quantity;
        existing.UnitPrice = basket.UnitPrice;
        existing.ProductId = basket.ProductId;
        existing.UserId = basket.UserId;
        existing.LegacyId = basket.LegacyId;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Baskets.FirstOrDefaultAsync(basket => basket.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _dbContext.Baskets.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}