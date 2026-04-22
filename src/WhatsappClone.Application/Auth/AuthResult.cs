namespace WhatsappClone.Application.Auth;

public sealed record AuthResult(
    string AccessToken,
    Guid UserId,
    string UserName,
    string Email);
