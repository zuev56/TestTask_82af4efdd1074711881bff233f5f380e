using MediatR;
using Api.Common.Domain.Interfaces;

namespace UserService.WebApi.Application.Features.Auth.Commands;

public sealed record LogoutUserCommand(string Token) : IRequest;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IJwtBlackListRepository _jwtTokenBlackListRepository;

    public LogoutUserCommandHandler(
        IJwtTokenService jwtTokenService,
        IJwtBlackListRepository jwtTokenBlackListRepository)
    {
        _jwtTokenService = jwtTokenService;
        _jwtTokenBlackListRepository = jwtTokenBlackListRepository;
    }

    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var expirationTime = _jwtTokenService.GetExpirationTime(request.Token);
        if (expirationTime < DateTime.UtcNow)
            return;

        if (await _jwtTokenBlackListRepository.ContainsAsync(request.Token, cancellationToken))
            return;

        await _jwtTokenBlackListRepository.AddAsync(request.Token, expirationTime, cancellationToken);
    }
}