using MediatR;
using Shared.Application.Interfaces;
using UserService.Application.Interfaces;

namespace UserService.Application.Features.Auth.Commands;

public sealed record LogoutUserCommand(string Token) : IRequest;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
{
    private readonly IJwtService _jwtTokenService;
    private readonly IJwtBlackListService _jwtBlackListService;

    public LogoutUserCommandHandler(
        IJwtService jwtTokenService,
        IJwtBlackListService jwtBlackListService)
    {
        _jwtTokenService = jwtTokenService;
        _jwtBlackListService = jwtBlackListService;
    }

    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var expirationTime = _jwtTokenService.GetExpirationTime(request.Token);
        if (expirationTime < DateTime.UtcNow)
            return;

        if (await _jwtBlackListService.IsBlackListedAsync(request.Token, cancellationToken))
            return;

        await _jwtBlackListService.AddToBlackListAsync(request.Token, expirationTime, cancellationToken);
    }
}