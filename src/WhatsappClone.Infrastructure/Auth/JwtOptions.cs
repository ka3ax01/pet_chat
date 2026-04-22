namespace WhatsappClone.Infrastructure.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "WhatsappClone";
    public string Audience { get; set; } = "WhatsappClone";
    public string Secret { get; set; } = default!;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
}
