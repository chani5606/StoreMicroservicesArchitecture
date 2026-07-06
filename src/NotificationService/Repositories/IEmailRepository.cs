namespace NotificationService.Repositories;

public interface IEmailRepository
{
    Task SendWinnerEmailAsync(string toEmail, string winnerName, string productName, CancellationToken cancellationToken = default);
}