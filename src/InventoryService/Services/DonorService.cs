using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories;

namespace InventoryService.Services;

public sealed class DonorService : IDonorService
{
    private readonly IDonorRepository _donorRepository;

    public DonorService(IDonorRepository donorRepository)
    {
        _donorRepository = donorRepository;
    }

    public async Task<IReadOnlyList<DonorResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var donors = await _donorRepository.GetAllAsync(cancellationToken);
        return donors.Select(Map).ToList();
    }

    public async Task<DonorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var donor = await _donorRepository.GetByIdAsync(id, cancellationToken);
        return donor is null ? null : Map(donor);
    }

    public async Task<DonorResponse> CreateAsync(DonorCreateRequest request, CancellationToken cancellationToken = default)
    {
        var donor = new Donor
        {
            LegacyId = request.LegacyId,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            City = request.City,
            Neighborhood = request.Neighborhood,
            Street = request.Street
        };

        var created = await _donorRepository.CreateAsync(donor, cancellationToken);
        return Map(created);
    }

    public async Task<DonorResponse?> UpdateAsync(int id, DonorUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _donorRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = request.Name;
        existing.Email = request.Email;
        existing.Phone = request.Phone;
        existing.City = request.City;
        existing.Neighborhood = request.Neighborhood;
        existing.Street = request.Street;

        var updated = await _donorRepository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _donorRepository.DeleteAsync(id, cancellationToken);
    }

    private static DonorResponse Map(Donor donor)
    {
        return new DonorResponse(
            donor.Id,
            donor.LegacyId,
            donor.Name,
            donor.Email,
            donor.Phone,
            donor.City,
            donor.Neighborhood,
            donor.Street);
    }
}