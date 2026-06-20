using MediatR;
using Shared.Domain.Exceptions;
using UserService.Application.DTO;
using UserService.Application.Interfaces;

namespace UserService.Application.Features.Users.Queries;

public sealed record GetUserQuery(int Id) : IRequest<UserResponse>;

public sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(query.Id, cancellationToken);
        if (user == null)
            throw new NotFoundException($"User with Id='{query.Id}' not found.");

        return new UserResponse(user.Id, user.Name);
    }
}