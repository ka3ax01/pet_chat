namespace WhatsappClone.Contracts.Auth;

public sealed record RegisterRequest(string UserName, string Email, string Password);

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(string AccessToken, Guid UserId, string UserName, string Email);

public sealed record CurrentUserResponse(Guid UserId, string UserName, string Email, string? AvatarUrl);
