namespace NotificationService.DTOs;

public sealed record WinnerCreateRequest(
    int LegacyId,
    int ProductId,
    int UserId,
    string ProductName,
    string WinnerName,
    string UserEmail);

public sealed record WinnerResponse(
    int Id,
    int LegacyId,
    int ProductId,
    int UserId,
    string ProductName,
    string WinnerName,
    string UserEmail,
    DateTime WonAtUtc);