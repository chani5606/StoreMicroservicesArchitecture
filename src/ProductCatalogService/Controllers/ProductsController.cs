using Microsoft.AspNetCore.Mvc;
using ProductCatalogService.DTOs;
using ProductCatalogService.Services;
using System.Net;

namespace ProductCatalogService.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        Response.Headers["X-Service-Instance"] = Environment.MachineName;
        Response.Headers["X-Service-Host"] = Dns.GetHostName();

        var product = await _productService.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("by-legacy/{legacyId:int}")]
    public async Task<ActionResult<ProductResponse>> GetByLegacyId(int legacyId, CancellationToken cancellationToken)
    {
        Response.Headers["X-Service-Instance"] = Environment.MachineName;
        Response.Headers["X-Service-Host"] = Dns.GetHostName();

        var product = await _productService.GetByLegacyIdAsync(legacyId, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(ProductCreateRequest request, CancellationToken cancellationToken)
    {
        var created = await _productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponse>> Update(string id, ProductUpdateRequest request, CancellationToken cancellationToken)
    {
        var updated = await _productService.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await _productService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}