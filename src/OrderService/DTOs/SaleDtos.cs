namespace OrderService.DTOs;

public sealed record SaleCreateRequest(int LegacyId, string Name, DateTime StartDateUtc, DateTime EndDateUtc);

public sealed record SaleUpdateRequest(string Name, DateTime StartDateUtc, DateTime EndDateUtc);

public sealed record SaleResponse(int Id, int LegacyId, string Name, DateTime StartDateUtc, DateTime EndDateUtc);