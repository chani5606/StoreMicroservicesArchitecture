namespace ProductCatalogService.DTOs;

public sealed record CategoryCreateRequest(int LegacyId, string Name);

public sealed record CategoryUpdateRequest(string Name);

public sealed record CategoryResponse(string Id, int LegacyId, string Name);