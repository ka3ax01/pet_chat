using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatsappClone.Application.Abstractions.Logging;
using WhatsappClone.Application.Abstractions.Persistence;
using WhatsappClone.Application.Abstractions.Services;
using WhatsappClone.Infrastructure.Logging;
using WhatsappClone.Infrastructure.Persistence;
using WhatsappClone.Infrastructure.Services;

namespace WhatsappClone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=whatsapp_clone;Username=postgres;Password=postgres";

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IBackendLogService, BackendLogService>();

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}
