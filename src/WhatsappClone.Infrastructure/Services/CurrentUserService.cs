using Microsoft.AspNetCore.Http;
using WhatsappClone.Application.Abstractions.Services;

namespace WhatsappClone.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public string UserName { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user is not null)
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "id");
            var userNameClaim = user.Claims.FirstOrDefault(c => c.Type == "username");

            if (userIdClaim is not null)
            {
                UserId = Guid.Parse(userIdClaim.Value);
            }

            if (userNameClaim is not null)
            {
                UserName = userNameClaim.Value;
            }
        }
    }
}
