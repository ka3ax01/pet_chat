using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhatsappClone.Application.Abstractions.Services;

namespace WhatsappClone.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly IDateTimeProvider _dateTimeProvider;

    private ILoggerFactory? _loggerFactory;

    private ICurrentUserService? _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
