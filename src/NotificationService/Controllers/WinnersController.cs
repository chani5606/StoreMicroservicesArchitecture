using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs;
using NotificationService.Services;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/winners")]
public sealed class WinnersController : ControllerBase
{
    private readonly IWinnerService _winnerService;

    public WinnersController(IWinnerService winnerService)
    {
        _winnerService = winnerService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WinnerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _winnerService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WinnerResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var winner = await _winnerService.GetByIdAsync(id, cancellationToken);
        return winner is null ? NotFound() : Ok(winner);
    }

    [HttpPost]
    public async Task<ActionResult<WinnerResponse>> Create(WinnerCreateRequest request, CancellationToken cancellationToken)
    {
        var created = await _winnerService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}