using Api.Common.Domain.Exceptions;
using MediatR;
using UserService.WebApi.Application.DTO;
using UserService.WebApi.Domain.Interfaces;

namespace UserService.WebApi.Application.Features.Users.Queries;

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