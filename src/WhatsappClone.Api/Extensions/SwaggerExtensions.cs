namespace WhatsappClone.Api.Extensions;

using Microsoft.OpenApi.Models;

public static class SwaggerExtensions
{
    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "WhatsappClone API",
                    Version = "v1",
                    Description = "HTTP API for WhatsappClone authentication, admin logs, chats, messages, and files."
                });

            var bearerScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter a JWT bearer token. Example: Bearer eyJhbGciOi...",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", bearerScheme);
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    [bearerScheme] = []
                });
        });

        return services;
    }

    public static WebApplication UseApiSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "WhatsappClone API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "WhatsappClone API";
        });

        return app;
    }
}
