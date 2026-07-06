using NotificationService.DTOs;

namespace NotificationService.Services;

public interface IEmailService
{
    Task SendWinnerEmailAsync(WinnerEmailRequest request, CancellationToken cancellationToken = default);

    Task<TotalIncomeResponse> GetTotalIncomeAsync(CancellationToken cancellationToken = default);
}