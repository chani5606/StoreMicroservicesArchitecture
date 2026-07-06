namespace NotificationService.Models;

public sealed class Winner
{
    public int Id { get; set; }

    public int LegacyId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string WinnerName { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;

    public DateTime WonAtUtc { get; set; } = DateTime.UtcNow;
}