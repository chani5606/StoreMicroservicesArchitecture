using MongoDB.Driver;
using ProductCatalogService.Data;
using ProductCatalogService.Models;

namespace ProductCatalogService.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<CategoryDocument> _categories;

    public CategoryRepository(CatalogMongoContext context)
    {
        _categories = context.Categories;
    }

    public async Task<CategoryDocument> CreateAsync(CategoryDocument category, CancellationToken cancellationToken = default)
    {
        await _categories.InsertOneAsync(category, cancellationToken: cancellationToken);
        return category;
    }

    public async Task<IReadOnlyList<CategoryDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _categories.Find(FilterDefinition<CategoryDocument>.Empty).ToListAsync(cancellationToken);
    }

    public async Task<CategoryDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _categories.Find(category => category.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryDocument?> GetByLegacyIdAsync(int legacyId, CancellationToken cancellationToken = default)
    {
        return await _categories.Find(category => category.LegacyId == legacyId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryDocument?> UpdateAsync(CategoryDocument category, CancellationToken cancellationToken = default)
    {
        var result = await _categories.ReplaceOneAsync(
            existing => existing.Id == category.Id,
            category,
            cancellationToken: cancellationToken);

        return result.MatchedCount == 0 ? null : category;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _categories.DeleteOneAsync(category => category.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }
}