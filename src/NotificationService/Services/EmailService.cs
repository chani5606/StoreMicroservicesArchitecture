using NotificationService.DTOs;
using NotificationService.Repositories;

namespace NotificationService.Services;

public sealed class EmailService : IEmailService
{
    private readonly IEmailRepository _emailRepository;

    public EmailService(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    public Task SendWinnerEmailAsync(WinnerEmailRequest request, CancellationToken cancellationToken = default)
    {
        return _emailRepository.SendWinnerEmailAsync(request.ToEmail, request.WinnerName, request.ProductName, cancellationToken);
    }

    public Task<TotalIncomeResponse> GetTotalIncomeAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TotalIncomeResponse(0m));
    }
}