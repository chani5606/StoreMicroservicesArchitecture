using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services;

public sealed class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;

    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<IReadOnlyList<SaleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepository.GetAllAsync(cancellationToken);
        return sales.Select(Map).ToList();
    }

    public async Task<SaleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
        return sale is null ? null : Map(sale);
    }

    public async Task<SaleResponse> CreateAsync(SaleCreateRequest request, CancellationToken cancellationToken = default)
    {
        var sale = new Sale
        {
            LegacyId = request.LegacyId,
            Name = request.Name,
            StartDateUtc = request.StartDateUtc,
            EndDateUtc = request.EndDateUtc
        };

        var created = await _saleRepository.CreateAsync(sale, cancellationToken);
        return Map(created);
    }

    public async Task<SaleResponse?> UpdateAsync(int id, SaleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _saleRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = request.Name;
        existing.StartDateUtc = request.StartDateUtc;
        existing.EndDateUtc = request.EndDateUtc;

        var updated = await _saleRepository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _saleRepository.DeleteAsync(id, cancellationToken);
    }

    public Task<bool> IsSaleOpenAsync(CancellationToken cancellationToken = default)
    {
        return _saleRepository.IsSaleOpenAsync(DateTime.UtcNow, cancellationToken);
    }

    private static SaleResponse Map(Sale sale)
    {
        return new SaleResponse(sale.Id, sale.LegacyId, sale.Name, sale.StartDateUtc, sale.EndDateUtc);
    }
}