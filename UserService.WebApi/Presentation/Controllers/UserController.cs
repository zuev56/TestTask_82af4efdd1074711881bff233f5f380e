using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.WebApi.Application.DTO;
using UserService.WebApi.Application.Features.Users.Queries;

namespace UserService.WebApi.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetUserAsync(int userId, CancellationToken cancellationToken)
    {
        var command = new GetUserQuery(userId);
        var user = await _mediator.Send(command, cancellationToken);

        return Ok(user);
    }
}