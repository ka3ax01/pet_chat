using WhatsappClone.Domain.Common;

namespace WhatsappClone.Domain.Entities;

public class ChatMember : IEntity
{
    public Guid ChatId { get; set; }

    public Guid UserId { get; set; }

    #region Navigation Properties
    public User User { get; set; } = default!;
    public Chat Chat { get; set; } = default!;
    #endregion
}
