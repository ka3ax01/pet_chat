using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using WhatsappClone.Infrastructure.Auth;

namespace WhatsappClone.IntegrationTests.Auth;

public class JwtProviderTests
{
    [Fact]
    public void GenerateAccessToken_IncludesUserClaimsAndRoles()
    {
        var options = Options.Create(
            new JwtOptions
            {
                Issuer = "WhatsappClone",
                Audience = "WhatsappClone",
                Secret = "TEST_SECRET_WITH_AT_LEAST_32_CHARACTERS",
                AccessTokenExpirationMinutes = 30
            });
        var provider = new JwtProvider(options);
        var userId = Guid.NewGuid();

        var token = provider.GenerateAccessToken(userId, "admin-user", ["Admin"]);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("WhatsappClone", jwt.Issuer);
        Assert.Equal("WhatsappClone", jwt.Audiences.Single());
        Assert.Contains(jwt.Claims, x => x.Type == "id" && x.Value == userId.ToString());
        Assert.Contains(jwt.Claims, x => x.Type == "username" && x.Value == "admin-user");
        Assert.Contains(jwt.Claims, x => x.Type == ClaimTypes.Role && x.Value == "Admin");
    }
}
