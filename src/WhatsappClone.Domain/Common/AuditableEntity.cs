namespace WhatsappClone.Domain.Common;

public class AuditableEntity<Tid> : BaseEntity<Tid>
    where Tid : notnull
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
}
