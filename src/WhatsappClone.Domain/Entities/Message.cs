using WhatsappClone.Domain.Common;
using WhatsappClone.Domain.Enums;

namespace WhatsappClone.Domain.Entities;

public class Message : AuditableEntity<Guid>
{
    public Guid ChatId { get; set; }
    public MessageType Type { get; set; }
    public string? Text { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public bool IsDeleted { get; set; }

    public Chat Chat { get; set; } = default!;
    public User CreatedBy { get; set; } = default!;
    public ICollection<MessageAttachment>? Attachments { get; set; } = new List<MessageAttachment>();
}
