namespace InventoryService.DTOs;

public sealed record DonorCreateRequest(
    int LegacyId,
    string Name,
    string Email,
    string Phone,
    string City,
    string Neighborhood,
    string Street);

public sealed record DonorUpdateRequest(
    string Name,
    string Email,
    string Phone,
    string City,
    string Neighborhood,
    string Street);

public sealed record DonorResponse(
    int Id,
    int LegacyId,
    string Name,
    string Email,
    string Phone,
    string City,
    string Neighborhood,
    string Street);