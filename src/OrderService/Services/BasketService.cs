using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services;

public sealed class BasketService : IBasketService
{
    private readonly IBasketRepository _basketRepository;

    public BasketService(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<IReadOnlyList<BasketResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var baskets = await _basketRepository.GetAllAsync(cancellationToken);
        return baskets.Select(Map).ToList();
    }

    public async Task<BasketResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetByIdAsync(id, cancellationToken);
        return basket is null ? null : Map(basket);
    }

    public async Task<BasketResponse> CreateAsync(BasketCreateRequest request, CancellationToken cancellationToken = default)
    {
        var basket = new Basket
        {
            CorrelationId = Guid.NewGuid(),
            LegacyId = request.LegacyId,
            UserId = request.UserId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            Status = "Pending",
            CreatedAtUtc = DateTime.UtcNow
        };

        var created = await _basketRepository.CreateAsync(basket, cancellationToken);
        return Map(created);
    }

    public async Task<BasketResponse?> UpdateStatusAsync(int id, BasketUpdateStatusRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _basketRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Status = request.Status;

        var updated = await _basketRepository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _basketRepository.DeleteAsync(id, cancellationToken);
    }

    private static BasketResponse Map(Basket basket)
    {
        return new BasketResponse(
            basket.Id,
            basket.CorrelationId,
            basket.LegacyId,
            basket.UserId,
            basket.ProductId,
            basket.Quantity,
            basket.Status,
            basket.UnitPrice,
            basket.CreatedAtUtc);
    }
}