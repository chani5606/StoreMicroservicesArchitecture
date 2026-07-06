using System.Net;
using System.Net.Mail;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService.Repositories;

public sealed class EmailRepository : IEmailRepository
{
    private readonly IConfiguration _configuration;
    private readonly NotificationDbContext _dbContext;

    public EmailRepository(IConfiguration configuration, NotificationDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task SendWinnerEmailAsync(string toEmail, string winnerName, string productName, CancellationToken cancellationToken = default)
    {
        var subject = "Congratulations! You won";
        var body = $"Hello {winnerName}, congratulations, you won {productName}.";

        using var message = new MailMessage
        {
            From = new MailAddress(_configuration["Smtp:Email"] ?? "yourproject@gmail.com"),
            Subject = subject,
            Body = body
        };

        message.To.Add(toEmail);

        using var smtpClient = new SmtpClient(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"] ?? "587"))
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _configuration["Smtp:Email"],
                _configuration["Smtp:Password"])
        };

        await smtpClient.SendMailAsync(message, cancellationToken);

        _dbContext.EmailLogs.Add(new EmailLog
        {
            RecipientEmail = toEmail,
            Subject = subject,
            Body = body,
            SentAtUtc = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}