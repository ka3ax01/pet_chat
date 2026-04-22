using Microsoft.AspNetCore.Http;
using System.Security.Claims;
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
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);
            var userNameClaim = user.Claims.FirstOrDefault(c => c.Type == "username" || c.Type == ClaimTypes.Name);

            if (userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                UserId = userId;
            }

            if (userNameClaim is not null)
            {
                UserName = userNameClaim.Value;
            }
        }
    }
}
