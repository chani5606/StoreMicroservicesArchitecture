using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;
using MassTransit;
using Shared.Contracts;

namespace OrderService.Controllers;

[ApiController]
[Route("api/baskets")]
public sealed class BasketsController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly IPublishEndpoint _publishEndpoint;

    public BasketsController(IBasketService basketService, IPublishEndpoint publishEndpoint)
    {
        _basketService = basketService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BasketResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _basketService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BasketResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var basket = await _basketService.GetByIdAsync(id, cancellationToken);
        return basket is null ? NotFound() : Ok(basket);
    }

    [HttpPost]
    public async Task<ActionResult<BasketResponse>> Create(BasketCreateRequest request, CancellationToken cancellationToken)
    {
        var created = await _basketService.CreateAsync(request, cancellationToken);

        await _publishEndpoint.Publish(
            new OrderPlacedEvent(
                created.CorrelationId,
                created.Id,
                created.ProductId,
                created.Quantity,
                DateTime.UtcNow),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<BasketResponse>> UpdateStatus(int id, BasketUpdateStatusRequest request, CancellationToken cancellationToken)
    {
        var updated = await _basketService.UpdateStatusAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _basketService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}