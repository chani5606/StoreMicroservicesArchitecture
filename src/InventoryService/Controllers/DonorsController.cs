using InventoryService.DTOs;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/donors")]
public sealed class DonorsController : ControllerBase
{
    private readonly IDonorService _donorService;

    public DonorsController(IDonorService donorService)
    {
        _donorService = donorService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DonorResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _donorService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DonorResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var donor = await _donorService.GetByIdAsync(id, cancellationToken);
        return donor is null ? NotFound() : Ok(donor);
    }

    [HttpPost]
    public async Task<ActionResult<DonorResponse>> Create(DonorCreateRequest request, CancellationToken cancellationToken)
    {
        var created = await _donorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DonorResponse>> Update(int id, DonorUpdateRequest request, CancellationToken cancellationToken)
    {
        var updated = await _donorService.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _donorService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}