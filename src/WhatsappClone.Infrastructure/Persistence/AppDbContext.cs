using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WhatsappClone.Application.Abstractions.Persistence;
using WhatsappClone.Application.Abstractions.Services;
using WhatsappClone.Domain.Common;
using WhatsappClone.Domain.Entities;
using WhatsappClone.Domain.Enums;

namespace WhatsappClone.Infrastructure.Persistence;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IDateTimeProvider dateTimeProvider,
    ICurrentUserService currentUserService) : DbContext(options), IAppDbContext
{
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatMember> ChatMembers => Set<ChatMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<MessageRead> MessageReads => Set<MessageRead>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<EntityAction> EntityActions => Set<EntityAction>();
    public DbSet<BackendLog> BackendLogs => Set<BackendLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        return SaveChanges(acceptAllChangesOnSuccess: true);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditingAndActions();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditingAndActions();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditingAndActions()
    {
        ChangeTracker.DetectChanges();

        var now = _dateTimeProvider.Now;
        var currentUserId = _currentUserService.UserId == Guid.Empty ? (Guid?)null : _currentUserService.UserId;
        var pendingActions = new List<EntityAction>();

        foreach (var entry in ChangeTracker.Entries().Where(ShouldTrackEntry))
        {
            var originalState = entry.State;

            if (entry.Entity is AuditableEntity<Guid> auditableEntity)
            {
                ApplyAuditState(entry, auditableEntity, now, currentUserId);
            }

            var actionType = ResolveActionType(originalState);

            if (actionType is null)
            {
                continue;
            }

            pendingActions.Add(
                new EntityAction
                {
                    Id = Guid.NewGuid(),
                    EntityName = entry.Metadata.ClrType.Name,
                    EntityId = GetEntityId(entry),
                    ActionType = actionType.Value,
                    ChangesJson = BuildChangesJson(entry, actionType.Value),
                    PerformedAt = now,
                    PerformedById = currentUserId
                });
        }

        if (pendingActions.Count > 0)
        {
            EntityActions.AddRange(pendingActions);
        }
    }

    private static bool ShouldTrackEntry(EntityEntry entry)
    {
        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
            && entry.Entity is not EntityAction
            && entry.Entity is not BackendLog;
    }

    private static EntityActionType? ResolveActionType(EntityState state)
    {
        return state switch
        {
            EntityState.Added => EntityActionType.Created,
            EntityState.Modified => EntityActionType.Updated,
            EntityState.Deleted => EntityActionType.Deleted,
            _ => null
        };
    }

    private static string GetEntityId(EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();

        if (primaryKey is null)
        {
            return string.Empty;
        }

        var keyValues = primaryKey.Properties.ToDictionary(
            property => property.Name,
            property => entry.Property(property.Name).CurrentValue ?? entry.Property(property.Name).OriginalValue);

        return JsonSerializer.Serialize(keyValues);
    }

    private static string? BuildChangesJson(EntityEntry entry, EntityActionType actionType)
    {
        object payload = actionType switch
        {
            EntityActionType.Created => entry.CurrentValues.Properties.ToDictionary(
                property => property.Name,
                property => entry.CurrentValues[property]),
            EntityActionType.Updated => entry.Properties
                .Where(property => property.IsModified)
                .ToDictionary(
                    property => property.Metadata.Name,
                    property => new
                    {
                        OldValue = property.OriginalValue,
                        NewValue = property.CurrentValue
                    }),
            EntityActionType.Deleted => entry.OriginalValues.Properties.ToDictionary(
                property => property.Name,
                property => entry.OriginalValues[property]),
            _ => new Dictionary<string, object?>()
        };

        return JsonSerializer.Serialize(payload);
    }

    private static void ApplyAuditState(
        EntityEntry entry,
        AuditableEntity<Guid> auditableEntity,
        DateTimeOffset now,
        Guid? currentUserId)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                auditableEntity.CreatedAt = now;
                auditableEntity.UpdatedAt = now;
                auditableEntity.CreatedById = currentUserId ?? Guid.Empty;
                auditableEntity.UpdatedById = currentUserId;
                break;
            case EntityState.Modified:
                auditableEntity.UpdatedAt = now;
                auditableEntity.UpdatedById = currentUserId;
                entry.Property(nameof(AuditableEntity<Guid>.CreatedAt)).IsModified = false;
                entry.Property(nameof(AuditableEntity<Guid>.CreatedById)).IsModified = false;
                break;
            case EntityState.Deleted:
                entry.State = EntityState.Modified;
                auditableEntity.IsDeleted = true;
                auditableEntity.UpdatedAt = now;
                auditableEntity.UpdatedById = currentUserId;
                entry.Property(nameof(AuditableEntity<Guid>.CreatedAt)).IsModified = false;
                entry.Property(nameof(AuditableEntity<Guid>.CreatedById)).IsModified = false;
                break;
        }
    }
}
