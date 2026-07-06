using MongoDB.Driver;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ProductCatalogService.Data;
using ProductCatalogService.Models;

namespace ProductCatalogService.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<ProductDocument> _products;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ProductRepository> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ProductRepository(CatalogMongoContext context, IDistributedCache cache, ILogger<ProductRepository> logger)
    {
        _products = context.Products;
        _cache = cache;
        _logger = logger;
    }

    private static string GetProductCacheKey(string id) => $"product:{id}";

    public async Task<ProductDocument> CreateAsync(ProductDocument product, CancellationToken cancellationToken = default)
    {
        product.Stock = product.Stock < 0 ? 0 : product.Stock;
        product.UpdatedAtUtc = DateTime.UtcNow;
        await _products.InsertOneAsync(product, cancellationToken: cancellationToken);
        await _cache.RemoveAsync(GetProductCacheKey(product.Id), cancellationToken);
        return product;
    }

    public async Task<IReadOnlyList<ProductDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _products.Find(FilterDefinition<ProductDocument>.Empty).ToListAsync(cancellationToken);
    }

    public async Task<ProductDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProductCacheKey(id);
        var cachedProduct = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrWhiteSpace(cachedProduct))
        {
            _logger.LogInformation("Cache hit for product {ProductId}", id);
            return JsonSerializer.Deserialize<ProductDocument>(cachedProduct, JsonOptions);
        }

        _logger.LogInformation("Cache miss for product {ProductId}", id);

        var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
        if (product is null)
        {
            return null;
        }

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product, JsonOptions),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        return product;
    }

    public async Task<ProductDocument?> GetByLegacyIdAsync(int legacyId, CancellationToken cancellationToken = default)
    {
        return await _products.Find(product => product.LegacyId == legacyId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductDocument?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _products.Find(product => product.Name == name).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductDocument?> UpdateAsync(ProductDocument product, CancellationToken cancellationToken = default)
    {
        product.Stock = product.Stock < 0 ? 0 : product.Stock;
        product.UpdatedAtUtc = DateTime.UtcNow;

        var result = await _products.ReplaceOneAsync(
            existing => existing.Id == product.Id,
            product,
            cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            return null;
        }

        await _cache.RemoveAsync(GetProductCacheKey(product.Id), cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _products.DeleteOneAsync(product => product.Id == id, cancellationToken);
        if (result.DeletedCount > 0)
        {
            await _cache.RemoveAsync(GetProductCacheKey(id), cancellationToken);
        }

        return result.DeletedCount > 0;
    }
}