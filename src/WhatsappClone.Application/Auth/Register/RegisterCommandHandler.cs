using WhatsappClone.Application.Abstractions.Auth;
using WhatsappClone.Application.Abstractions.Persistence;
using WhatsappClone.Domain.Constants;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Application.Auth.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IAppDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider)
{
    public async Task<AuthResult> Handle(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var userName = command.UserName.Trim();
        var email = command.Email.Trim();

        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new InvalidOperationException("User name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < 8)
        {
            throw new InvalidOperationException("Password must contain at least 8 characters.");
        }

        if (await userRepository.ExistsByUserNameAsync(userName, cancellationToken))
        {
            throw new InvalidOperationException("User name is already taken.");
        }

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            PasswordHash = passwordHasher.Hash(command.Password),
            UserRoles =
            [
                new UserRole
                {
                    UserId = Guid.NewGuid(),
                    RoleId = SystemRoles.UserId
                }
            ]
        };

        user.UserRoles.First().UserId = user.Id;

        userRepository.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        var accessToken = jwtProvider.GenerateAccessToken(user.Id, user.UserName, [SystemRoles.User]);

        return new AuthResult(accessToken, user.Id, user.UserName, user.Email);
    }
}
