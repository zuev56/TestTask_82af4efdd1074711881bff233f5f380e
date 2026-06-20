using FinanceService.Application.DTO;
using FinanceService.Application.Features.Currency.Commands;
using FinanceService.Application.Features.Currency.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CurrencyController : Controller
{
    private readonly IMediator _mediator;

    public CurrencyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AllCurrenciesResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var command = new GetAllCurrenciesQuery();
        var currencies = await _mediator.Send(command, cancellationToken);

        return Ok(currencies);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserCurrenciesResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetForUserAsync(int userId, CancellationToken cancellationToken)
    {
        var command = new GetUserCurrenciesQuery(userId);
        var currencies = await _mediator.Send(command, cancellationToken);

        return Ok(currencies);
    }

    [HttpPost("{currencyId}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> AttachToUserAsync(int currencyId, int userId, CancellationToken cancellationToken)
    {
        var command = new AttachCurrencyToUserCommand(currencyId, userId);
        await _mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpDelete("{currencyId}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DetachFromUserAsync(int currencyId, int userId, CancellationToken cancellationToken)
    {
        var command = new DetachCurrencyFromUserCommand(currencyId, userId);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

}