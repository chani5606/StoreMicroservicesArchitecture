using NotificationService.DTOs;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Services;

public sealed class WinnerService : IWinnerService
{
    private readonly IWinnerRepository _winnerRepository;

    public WinnerService(IWinnerRepository winnerRepository)
    {
        _winnerRepository = winnerRepository;
    }

    public async Task<IReadOnlyList<WinnerResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var winners = await _winnerRepository.GetAllAsync(cancellationToken);
        return winners.Select(Map).ToList();
    }

    public async Task<WinnerResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var winner = await _winnerRepository.GetByIdAsync(id, cancellationToken);
        return winner is null ? null : Map(winner);
    }

    public async Task<WinnerResponse> CreateAsync(WinnerCreateRequest request, CancellationToken cancellationToken = default)
    {
        var winner = new Winner
        {
            LegacyId = request.LegacyId,
            ProductId = request.ProductId,
            UserId = request.UserId,
            ProductName = request.ProductName,
            WinnerName = request.WinnerName,
            UserEmail = request.UserEmail,
            WonAtUtc = DateTime.UtcNow
        };

        var created = await _winnerRepository.CreateAsync(winner, cancellationToken);
        return Map(created);
    }

    private static WinnerResponse Map(Winner winner)
    {
        return new WinnerResponse(
            winner.Id,
            winner.LegacyId,
            winner.ProductId,
            winner.UserId,
            winner.ProductName,
            winner.WinnerName,
            winner.UserEmail,
            winner.WonAtUtc);
    }
}