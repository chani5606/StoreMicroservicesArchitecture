using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;
using Shared.Contracts;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrderController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderController(IBasketService basketService, IPublishEndpoint publishEndpoint)
    {
        _basketService = basketService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<ActionResult<BasketResponse>> PlaceOrder(BasketCreateRequest request, CancellationToken cancellationToken)
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

        return CreatedAtAction(nameof(PlaceOrder), new { id = created.Id }, created);
    }
}
