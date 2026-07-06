using ProductCatalogService.DTOs;
using ProductCatalogService.Models;
using ProductCatalogService.Repositories;

namespace ProductCatalogService.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(Map).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : Map(product);
    }

    public async Task<ProductResponse?> GetByLegacyIdAsync(int legacyId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByLegacyIdAsync(legacyId, cancellationToken);
        return product is null ? null : Map(product);
    }

    public async Task<ProductResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default)
    {
        var product = new ProductDocument
        {
            LegacyId = request.LegacyId,
            Name = request.Name,
            GiftNumber = request.GiftNumber,
            Price = request.Price,
            Stock = request.Stock,
            PathImage = request.PathImage,
            CategoryId = request.CategoryId,
            CategoryName = request.CategoryName
        };

        var created = await _productRepository.CreateAsync(product, cancellationToken);
        return Map(created);
    }

    public async Task<ProductResponse?> UpdateAsync(string id, ProductUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = request.Name;
        existing.GiftNumber = request.GiftNumber;
        existing.Price = request.Price;
        existing.Stock = request.Stock;
        existing.PathImage = request.PathImage;
        existing.CategoryId = request.CategoryId;
        existing.CategoryName = request.CategoryName;

        var updated = await _productRepository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _productRepository.DeleteAsync(id, cancellationToken);
    }

    private static ProductResponse Map(ProductDocument product)
    {
        return new ProductResponse(
            product.Id,
            product.LegacyId,
            product.Name,
            product.GiftNumber,
            product.Price,
            product.Stock,
            product.PathImage,
            product.CategoryId,
            product.CategoryName,
            product.UpdatedAtUtc);
    }
}