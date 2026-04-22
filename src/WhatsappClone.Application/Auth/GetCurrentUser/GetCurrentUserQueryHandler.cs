using WhatsappClone.Application.Abstractions.Persistence;

namespace WhatsappClone.Application.Auth.GetCurrentUser;

public class GetCurrentUserQueryHandler(IUserRepository userRepository)
{
    public async Task<CurrentUserResult> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Current user was not found.");
        }

        return new CurrentUserResult(user.Id, user.UserName, user.Email, user.AvatarUrl);
    }
}

public sealed record CurrentUserResult(Guid UserId, string UserName, string Email, string? AvatarUrl);
