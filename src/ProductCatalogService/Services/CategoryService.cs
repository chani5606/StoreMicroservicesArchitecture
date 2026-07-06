using ProductCatalogService.DTOs;
using ProductCatalogService.Models;
using ProductCatalogService.Repositories;

namespace ProductCatalogService.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(Map).ToList();
    }

    public async Task<CategoryResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category is null ? null : Map(category);
    }

    public async Task<CategoryResponse> CreateAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        var category = new CategoryDocument
        {
            LegacyId = request.LegacyId,
            Name = request.Name
        };

        var created = await _categoryRepository.CreateAsync(category, cancellationToken);
        return Map(created);
    }

    public async Task<CategoryResponse?> UpdateAsync(string id, CategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = request.Name;

        var updated = await _categoryRepository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _categoryRepository.DeleteAsync(id, cancellationToken);
    }

    private static CategoryResponse Map(CategoryDocument category)
    {
        return new CategoryResponse(category.Id, category.LegacyId, category.Name);
    }
}