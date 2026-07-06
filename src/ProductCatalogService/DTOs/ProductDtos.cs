namespace ProductCatalogService.DTOs;

public sealed record ProductCreateRequest(
    int LegacyId,
    string Name,
    int GiftNumber,
    decimal Price,
    int Stock,
    string PathImage,
    string CategoryId,
    string CategoryName);

public sealed record ProductUpdateRequest(
    string Name,
    int GiftNumber,
    decimal Price,
    int Stock,
    string PathImage,
    string CategoryId,
    string CategoryName);

public sealed record ProductResponse(
    string Id,
    int LegacyId,
    string Name,
    int GiftNumber,
    decimal Price,
    int Stock,
    string PathImage,
    string CategoryId,
    string CategoryName,
    DateTime UpdatedAtUtc);