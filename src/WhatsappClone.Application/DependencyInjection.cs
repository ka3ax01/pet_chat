using Microsoft.Extensions.DependencyInjection;
using WhatsappClone.Application.Auth.GetCurrentUser;
using WhatsappClone.Application.Auth.Login;
using WhatsappClone.Application.Auth.Register;

namespace WhatsappClone.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterCommandHandler>();
        services.AddScoped<LoginQueryHandler>();
        services.AddScoped<GetCurrentUserQueryHandler>();

        return services;
    }
}
