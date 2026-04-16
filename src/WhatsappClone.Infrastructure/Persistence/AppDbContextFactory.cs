using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WhatsappClone.Application.Abstractions.Services;

namespace WhatsappClone.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = "Host=localhost;Port=5432;Database=whatsapp_clone;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options, new DesignTimeDateTimeProvider(), new DesignTimeCurrentUserService());
    }

    private sealed class DesignTimeDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }

    private sealed class DesignTimeCurrentUserService : ICurrentUserService
    {
        public string UserName => "design-time";

        public Guid UserId => Guid.Empty;
    }
}
