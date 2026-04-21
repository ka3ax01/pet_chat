using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WhatsappClone.Application.Abstractions.Services;
using WhatsappClone.Domain.Entities;
using WhatsappClone.Domain.Enums;
using WhatsappClone.Infrastructure.Persistence;

namespace WhatsappClone.IntegrationTests.Persistence;

public class AppDbContextTests
{
    [Fact]
    public async Task SaveChangesAsync_WhenEntityIsAdded_AppliesAuditFieldsAndCreatesEntityAction()
    {
        var now = new DateTimeOffset(2026, 4, 16, 10, 0, 0, TimeSpan.Zero);
        var currentUserId = Guid.NewGuid();
        await using var context = CreateContext(now, currentUserId);

        var user = CreateUser();

        context.Users.Add(user);
        await context.SaveChangesAsync();

        Assert.Equal(now, user.CreatedAt);
        Assert.Equal(now, user.UpdatedAt);
        Assert.Equal(currentUserId, user.CreatedById);
        Assert.Equal(currentUserId, user.UpdatedById);

        var action = await context.EntityActions.SingleAsync();
        Assert.Equal(nameof(User), action.EntityName);
        Assert.Equal(EntityActionType.Created, action.ActionType);
        Assert.Equal(now, action.PerformedAt);
        Assert.Equal(currentUserId, action.PerformedById);
        Assert.Contains(user.Id.ToString(), action.EntityId);
        Assert.Contains(nameof(User.Email), action.ChangesJson);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityIsModified_PreservesCreationAuditAndStoresChangedProperties()
    {
        var createdAt = new DateTimeOffset(2026, 4, 16, 10, 0, 0, TimeSpan.Zero);
        var updatedAt = createdAt.AddMinutes(5);
        var currentUserId = Guid.NewGuid();
        await using var context = CreateContext(createdAt, currentUserId);

        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();
        context.EntityActions.RemoveRange(context.EntityActions);
        await context.SaveChangesAsync();

        context.SetNow(updatedAt);
        user.Email = "updated@example.com";
        await context.SaveChangesAsync();

        Assert.Equal(createdAt, user.CreatedAt);
        Assert.Equal(currentUserId, user.CreatedById);
        Assert.Equal(updatedAt, user.UpdatedAt);
        Assert.Equal(currentUserId, user.UpdatedById);

        var action = await context.EntityActions.SingleAsync();
        Assert.Equal(EntityActionType.Updated, action.ActionType);

        using var changes = JsonDocument.Parse(action.ChangesJson!);
        var emailChange = changes.RootElement.GetProperty(nameof(User.Email));
        Assert.Equal("user@example.com", emailChange.GetProperty("OldValue").GetString());
        Assert.Equal("updated@example.com", emailChange.GetProperty("NewValue").GetString());
    }

    [Fact]
    public async Task SaveChangesAsync_WhenAuditableEntityIsDeleted_PerformsSoftDeleteAndCreatesDeletedAction()
    {
        var now = new DateTimeOffset(2026, 4, 16, 10, 0, 0, TimeSpan.Zero);
        var currentUserId = Guid.NewGuid();
        await using var context = CreateContext(now, currentUserId);

        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();
        context.EntityActions.RemoveRange(context.EntityActions);
        await context.SaveChangesAsync();

        var deletedAt = now.AddMinutes(10);
        context.SetNow(deletedAt);
        context.Users.Remove(user);
        await context.SaveChangesAsync();

        Assert.True(user.IsDeleted);
        Assert.Equal(deletedAt, user.UpdatedAt);
        Assert.Equal(currentUserId, user.UpdatedById);
        Assert.Equal(EntityState.Unchanged, context.Entry(user).State);
        Assert.True(await context.Users.AnyAsync(x => x.Id == user.Id));

        var action = await context.EntityActions.SingleAsync();
        Assert.Equal(nameof(User), action.EntityName);
        Assert.Equal(EntityActionType.Deleted, action.ActionType);
        Assert.Equal(deletedAt, action.PerformedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityActionIsAdded_DoesNotCreateNestedEntityAction()
    {
        var now = new DateTimeOffset(2026, 4, 16, 10, 0, 0, TimeSpan.Zero);
        await using var context = CreateContext(now, Guid.NewGuid());

        context.EntityActions.Add(
            new EntityAction
            {
                Id = Guid.NewGuid(),
                EntityName = nameof(User),
                EntityId = "{}",
                ActionType = EntityActionType.Created,
                PerformedAt = now
            });

        await context.SaveChangesAsync();

        Assert.Equal(1, await context.EntityActions.CountAsync());
    }

    private static TestAppDbContext CreateContext(DateTimeOffset now, Guid currentUserId)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestAppDbContext(options, new StubDateTimeProvider(now), new StubCurrentUserService(currentUserId));
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            UserName = "user",
            Email = "user@example.com",
            PasswordHash = "hash"
        };
    }

    private sealed class TestAppDbContext(
        DbContextOptions<AppDbContext> options,
        StubDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
        : AppDbContext(options, dateTimeProvider, currentUserService)
    {
        public void SetNow(DateTimeOffset now)
        {
            dateTimeProvider.Now = now;
        }
    }

    private sealed class StubDateTimeProvider(DateTimeOffset now) : IDateTimeProvider
    {
        public DateTimeOffset Now { get; set; } = now;
    }

    private sealed class StubCurrentUserService(Guid userId) : ICurrentUserService
    {
        public string UserName => "test-user";

        public Guid UserId => userId;
    }
}
