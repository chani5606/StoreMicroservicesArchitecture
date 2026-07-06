using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/sales")]
public sealed class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SaleResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _saleService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SaleResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var sale = await _saleService.GetByIdAsync(id, cancellationToken);
        return sale is null ? NotFound() : Ok(sale);
    }

    [HttpGet("is-open")]
    public async Task<ActionResult<bool>> IsOpen(CancellationToken cancellationToken)
    {
        return Ok(await _saleService.IsSaleOpenAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<SaleResponse>> Create(SaleCreateRequest request, CancellationToken cancellationToken)
    {
        var created = await _saleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SaleResponse>> Update(int id, SaleUpdateRequest request, CancellationToken cancellationToken)
    {
        var updated = await _saleService.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _saleService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}