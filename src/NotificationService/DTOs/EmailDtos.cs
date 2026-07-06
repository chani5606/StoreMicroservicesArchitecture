namespace NotificationService.DTOs;

public sealed record WinnerEmailRequest(string ToEmail, string WinnerName, string ProductName);

public sealed record TotalIncomeResponse(decimal TotalIncome);