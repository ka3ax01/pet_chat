using WhatsappClone.Domain.Common;

namespace WhatsappClone.Domain.Entities;

public class MessageRead : BaseEntity<Guid>
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset ReadAt { get; set; }

    #region Navigation Properties
    public Message Message { get; set; } = default!;
    public User User { get; set; } = default!;
    #endregion
}
