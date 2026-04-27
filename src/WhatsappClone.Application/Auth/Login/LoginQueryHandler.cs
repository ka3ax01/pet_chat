using WhatsappClone.Application.Abstractions.Auth;
using WhatsappClone.Application.Abstractions.Persistence;

namespace WhatsappClone.Application.Auth.Login;

public class LoginQueryHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider)
{
    public async Task<AuthResult> Handle(LoginQuery query, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(query.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(query.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var roles = user.UserRoles.Select(x => x.Role.Name).ToArray();
        var accessToken = jwtProvider.GenerateAccessToken(user.Id, user.UserName, roles);

        return new AuthResult(accessToken, user.Id, user.UserName, user.Email);
    }
}
