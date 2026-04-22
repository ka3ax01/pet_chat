namespace WhatsappClone.Application.Abstractions.Auth;

public interface IJwtProvider
{
    string GenerateAccessToken(Guid userId, string userName, IEnumerable<string>? roles = null);
}
