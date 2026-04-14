using WhatsappClone.Domain.Common;

namespace WhatsappClone.Domain.Entities;

public class MessageAttachment : AuditableEntity<Guid>
{
    public Guid MessageId { get; set; }
    public string FileName { get; set; } = default!;
    public string FileUrl { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }
    public long FileSizeBytes { get; set; }
    public bool IsDeleted { get; set; }

    #region Navigation Properties
    public Message Message { get; set; } = default!;

    public User CreatedBy { get; set; } = default!;

    #endregion
}
