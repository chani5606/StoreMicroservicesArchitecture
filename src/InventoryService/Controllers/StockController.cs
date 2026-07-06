using InventoryService.DTOs;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/stock")]
public sealed class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductStockResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _stockService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductStockResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var stock = await _stockService.GetByIdAsync(id, cancellationToken);
        return stock is null ? NotFound() : Ok(stock);
    }

    [HttpGet("by-product/{productId:int}")]
    public async Task<ActionResult<ProductStockResponse>> GetByProductId(int productId, CancellationToken cancellationToken)
    {
        var stock = await _stockService.GetByProductIdAsync(productId, cancellationToken);
        return stock is null ? NotFound() : Ok(stock);
    }

    [HttpPost]
    public async Task<ActionResult<ProductStockResponse>> Create(ProductStockCreateRequest request, CancellationToken cancellationToken)
    {
        var created = await _stockService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:int}/adjust")]
    public async Task<ActionResult<ProductStockResponse>> Adjust(int id, ProductStockAdjustRequest request, CancellationToken cancellationToken)
    {
        var updated = await _stockService.AdjustAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _stockService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}