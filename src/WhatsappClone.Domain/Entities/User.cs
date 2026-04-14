using WhatsappClone.Domain.Common;

namespace WhatsappClone.Domain.Entities;

public class User : AuditableEntity<Guid>
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }

    public ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();

    public ICollection<Chat> CreatedChats { get; set; } = new List<Chat>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
