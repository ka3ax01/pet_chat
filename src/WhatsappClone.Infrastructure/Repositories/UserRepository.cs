using Microsoft.EntityFrameworkCore;
using WhatsappClone.Application.Abstractions.Persistence;
using WhatsappClone.Domain.Entities;
using WhatsappClone.Infrastructure.Persistence;

namespace WhatsappClone.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .AsNoTracking()
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(email);

        return dbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail && !x.IsDeleted, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(email);

        return dbContext.Users
            .AnyAsync(x => x.Email.ToLower() == normalizedEmail && !x.IsDeleted, cancellationToken);
    }

    public Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var normalizedUserName = Normalize(userName);

        return dbContext.Users
            .AnyAsync(x => x.UserName.ToLower() == normalizedUserName && !x.IsDeleted, cancellationToken);
    }

    public void Add(User user)
    {
        dbContext.Users.Add(user);
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
