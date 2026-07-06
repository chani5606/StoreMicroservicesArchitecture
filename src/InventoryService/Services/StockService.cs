using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories;

namespace InventoryService.Services;

public sealed class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;

    public StockService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<IReadOnlyList<ProductStockResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var stocks = await _stockRepository.GetAllAsync(cancellationToken);
        return stocks.Select(Map).ToList();
    }

    public async Task<ProductStockResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var stock = await _stockRepository.GetByIdAsync(id, cancellationToken);
        return stock is null ? null : Map(stock);
    }

    public async Task<ProductStockResponse?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId, cancellationToken);
        return stock is null ? null : Map(stock);
    }

    public async Task<ProductStockResponse> CreateAsync(ProductStockCreateRequest request, CancellationToken cancellationToken = default)
    {
        var stock = new ProductStock
        {
            ProductId = request.ProductId,
            DonorId = request.DonorId,
            QuantityAvailable = request.QuantityAvailable,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var created = await _stockRepository.CreateAsync(stock, cancellationToken);
        return Map(created);
    }

    public async Task<ProductStockResponse?> AdjustAsync(int id, ProductStockAdjustRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _stockRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.QuantityAvailable += request.QuantityDelta;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        var updated = await _stockRepository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _stockRepository.DeleteAsync(id, cancellationToken);
    }

    private static ProductStockResponse Map(ProductStock stock)
    {
        return new ProductStockResponse(
            stock.Id,
            stock.ProductId,
            stock.DonorId,
            stock.QuantityAvailable,
            stock.UpdatedAtUtc);
    }
}