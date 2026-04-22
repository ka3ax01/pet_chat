using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatsappClone.Application.Abstractions.Auth;
using WhatsappClone.Application.Abstractions.Logging;
using WhatsappClone.Application.Abstractions.Persistence;
using WhatsappClone.Application.Abstractions.Services;
using WhatsappClone.Infrastructure.Auth;
using WhatsappClone.Infrastructure.Logging;
using WhatsappClone.Infrastructure.Persistence;
using WhatsappClone.Infrastructure.Repositories;
using WhatsappClone.Infrastructure.Services;

namespace WhatsappClone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=whatsapp_clone;Username=postgres;Password=postgres";

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, Services.CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.Configure<JwtOptions>(options =>
        {
            var section = configuration.GetSection(JwtOptions.SectionName);

            options.Issuer = section[nameof(JwtOptions.Issuer)] ?? options.Issuer;
            options.Audience = section[nameof(JwtOptions.Audience)] ?? options.Audience;
            options.Secret = section[nameof(JwtOptions.Secret)] ?? options.Secret;

            if (int.TryParse(section[nameof(JwtOptions.AccessTokenExpirationMinutes)], out var expirationMinutes))
            {
                options.AccessTokenExpirationMinutes = expirationMinutes;
            }
        });
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IBackendLogService, BackendLogService>();

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
