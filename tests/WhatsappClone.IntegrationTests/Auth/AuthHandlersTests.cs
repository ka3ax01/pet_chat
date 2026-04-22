using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhatsappClone.Application.Abstractions.Services;
using WhatsappClone.Application.Auth.GetCurrentUser;
using WhatsappClone.Application.Auth.Login;
using WhatsappClone.Application.Auth.Register;
using WhatsappClone.Infrastructure.Auth;
using WhatsappClone.Infrastructure.Persistence;
using WhatsappClone.Infrastructure.Repositories;

namespace WhatsappClone.IntegrationTests.Auth;

public class AuthHandlersTests
{
    [Fact]
    public async Task RegisterCommand_CreatesUserAndReturnsAccessToken()
    {
        await using var context = CreateContext();
        var userRepository = new UserRepository(context);
        var handler = new RegisterCommandHandler(userRepository, context, new PasswordHasher(), CreateJwtProvider());

        var result = await handler.Handle(new RegisterCommand("new-user", "new@example.com", "password123"));

        Assert.NotEmpty(result.AccessToken);
        Assert.Equal("new-user", result.UserName);
        Assert.Equal("new@example.com", result.Email);
        Assert.True(await context.Users.AnyAsync(x => x.Id == result.UserId));
        Assert.NotEqual("password123", (await context.Users.SingleAsync()).PasswordHash);
    }

    [Fact]
    public async Task RegisterCommand_WhenEmailExists_Throws()
    {
        await using var context = CreateContext();
        var userRepository = new UserRepository(context);
        var handler = new RegisterCommandHandler(userRepository, context, new PasswordHasher(), CreateJwtProvider());

        await handler.Handle(new RegisterCommand("first", "same@example.com", "password123"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new RegisterCommand("second", "same@example.com", "password123")));
    }

    [Fact]
    public async Task LoginQuery_WithValidCredentials_ReturnsAccessToken()
    {
        await using var context = CreateContext();
        var userRepository = new UserRepository(context);
        var passwordHasher = new PasswordHasher();
        var jwtProvider = CreateJwtProvider();
        var registerHandler = new RegisterCommandHandler(userRepository, context, passwordHasher, jwtProvider);
        var loginHandler = new LoginQueryHandler(userRepository, passwordHasher, jwtProvider);

        await registerHandler.Handle(new RegisterCommand("new-user", "new@example.com", "password123"));

        var result = await loginHandler.Handle(new LoginQuery("new@example.com", "password123"));

        Assert.NotEmpty(result.AccessToken);
        Assert.Equal("new-user", result.UserName);
    }

    [Fact]
    public async Task LoginQuery_WithInvalidPassword_ThrowsUnauthorized()
    {
        await using var context = CreateContext();
        var userRepository = new UserRepository(context);
        var passwordHasher = new PasswordHasher();
        var jwtProvider = CreateJwtProvider();
        var registerHandler = new RegisterCommandHandler(userRepository, context, passwordHasher, jwtProvider);
        var loginHandler = new LoginQueryHandler(userRepository, passwordHasher, jwtProvider);

        await registerHandler.Handle(new RegisterCommand("new-user", "new@example.com", "password123"));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => loginHandler.Handle(new LoginQuery("new@example.com", "wrong-password")));
    }

    [Fact]
    public async Task GetCurrentUserQuery_ReturnsCurrentUserProfile()
    {
        await using var context = CreateContext();
        var userRepository = new UserRepository(context);
        var registerHandler = new RegisterCommandHandler(userRepository, context, new PasswordHasher(), CreateJwtProvider());
        var meHandler = new GetCurrentUserQueryHandler(userRepository);
        var authResult = await registerHandler.Handle(new RegisterCommand("new-user", "new@example.com", "password123"));

        var result = await meHandler.Handle(new GetCurrentUserQuery(authResult.UserId));

        Assert.Equal(authResult.UserId, result.UserId);
        Assert.Equal("new-user", result.UserName);
        Assert.Equal("new@example.com", result.Email);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, new StubDateTimeProvider(), new StubCurrentUserService());
    }

    private static JwtProvider CreateJwtProvider()
    {
        return new JwtProvider(
            Options.Create(
                new JwtOptions
                {
                    Issuer = "WhatsappClone",
                    Audience = "WhatsappClone",
                    Secret = "TEST_SECRET_WITH_AT_LEAST_32_CHARACTERS",
                    AccessTokenExpirationMinutes = 30
                }));
    }

    private sealed class StubDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now { get; } = new(2026, 4, 21, 10, 0, 0, TimeSpan.Zero);
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public string UserName => string.Empty;

        public Guid UserId => Guid.Empty;
    }
}
