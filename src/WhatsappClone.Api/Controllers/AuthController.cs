using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WhatsappClone.Application.Abstractions.Auth;
using WhatsappClone.Application.Auth.GetCurrentUser;
using WhatsappClone.Application.Auth.Login;
using WhatsappClone.Application.Auth.Register;
using WhatsappClone.Contracts.Auth;

namespace WhatsappClone.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IJwtProvider jwtProvider,
    IWebHostEnvironment environment,
    RegisterCommandHandler registerCommandHandler,
    LoginQueryHandler loginQueryHandler,
    GetCurrentUserQueryHandler getCurrentUserQueryHandler) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await registerCommandHandler.Handle(
                new RegisterCommand(request.UserName, request.Email, request.Password),
                cancellationToken);

            return Ok(new AuthResponse(result.AccessToken, result.UserId, result.UserName, result.Email));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await loginQueryHandler.Handle(
                new LoginQuery(request.Email, request.Password),
                cancellationToken);

            return Ok(new AuthResponse(result.AccessToken, result.UserId, result.UserName, result.Email));
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new { error = exception.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "id");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = "Invalid access token." });
        }

        try
        {
            var result = await getCurrentUserQueryHandler.Handle(new GetCurrentUserQuery(userId), cancellationToken);
            return Ok(new CurrentUserResponse(result.UserId, result.UserName, result.Email, result.AvatarUrl));
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new { error = exception.Message });
        }
    }

    [HttpPost("dev-token")]
    public ActionResult<DevTokenResponse> CreateDevToken([FromBody] DevTokenRequest request)
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        var userId = request.UserId ?? Guid.NewGuid();
        var userName = string.IsNullOrWhiteSpace(request.UserName) ? "dev-admin" : request.UserName;
        var roles = request.Roles is null || request.Roles.Count == 0 ? ["Admin"] : request.Roles;
        var accessToken = jwtProvider.GenerateAccessToken(userId, userName, roles);

        return Ok(new DevTokenResponse(accessToken, userId, userName, roles));
    }
}

public sealed record DevTokenRequest(Guid? UserId, string? UserName, IReadOnlyCollection<string>? Roles);

public sealed record DevTokenResponse(string AccessToken, Guid UserId, string UserName, IReadOnlyCollection<string> Roles);
