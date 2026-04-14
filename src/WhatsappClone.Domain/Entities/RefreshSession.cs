using WhatsappClone.Domain.Common;

namespace WhatsappClone.Domain.Entities;

public class RefreshSession : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public string RefreshTokenHash { get; set; } = default!;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    #region Navigation Properties
    public User User { get; set; } = default!;
    #endregion
}
