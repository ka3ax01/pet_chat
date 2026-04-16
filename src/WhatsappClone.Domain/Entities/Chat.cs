using WhatsappClone.Domain.Common;
using WhatsappClone.Domain.Enums;

namespace WhatsappClone.Domain.Entities;

public class Chat : AuditableEntity<Guid>
{
    public ChatType Type { get; set; }
    public string? Title { get; set; }
    public string? PhotoUrl { get; set; }

    #region Navigation Properties
    public User CreatedBy { get; set; } = default!;
    public ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    #endregion
}
