using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Common;
using UserService.WebApi.Application.DTO;
using UserService.WebApi.Application.Features.Auth.Commands;

namespace UserService.WebApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Name, request.Password);
        var user = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(Register), user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Name, request.Password);
        var result = await _mediator.Send(command, cancellationToken);

        Response.Cookies.Append("Authorization", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !EnvironmentUtils.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = result.ValidTo
        });

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var token = Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrWhiteSpace(token))
            token = Request.Cookies["Authorization"];

        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new ProblemDetails
            {
                Title = "Token missing",
                Detail = "The authentication token could not be found in the request."
            });

        var command = new LogoutUserCommand(token);
        await _mediator.Send(command, cancellationToken);

        if (Request.Cookies.ContainsKey("Authorization"))
            Response.Cookies.Delete("Authorization");

        return NoContent();
    }
}