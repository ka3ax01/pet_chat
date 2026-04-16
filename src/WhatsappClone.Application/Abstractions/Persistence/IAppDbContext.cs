using Microsoft.EntityFrameworkCore;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Application.Abstractions.Persistence;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Chat> Chats { get; }
    DbSet<ChatMember> ChatMembers { get; }
    DbSet<Message> Messages { get; }
    DbSet<MessageAttachment> MessageAttachments { get; }
    DbSet<MessageRead> MessageReads { get; }
    DbSet<RefreshSession> RefreshSessions { get; }
    DbSet<EntityAction> EntityActions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
