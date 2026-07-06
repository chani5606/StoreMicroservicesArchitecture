using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs;
using NotificationService.Services;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IEmailService _emailService;

    public ReportsController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("winner-email")]
    public async Task<ActionResult> SendWinnerEmail(WinnerEmailRequest request, CancellationToken cancellationToken)
    {
        await _emailService.SendWinnerEmailAsync(request, cancellationToken);
        return Accepted();
    }

    [HttpGet("total-income")]
    public async Task<ActionResult<TotalIncomeResponse>> GetTotalIncome(CancellationToken cancellationToken)
    {
        return Ok(await _emailService.GetTotalIncomeAsync(cancellationToken));
    }
}