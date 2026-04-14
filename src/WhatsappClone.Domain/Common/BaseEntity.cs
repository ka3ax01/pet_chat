namespace WhatsappClone.Domain.Common;

public class BaseEntity<Tid>
    where Tid : notnull
{
    public Tid Id { get; set; } = default!;
}
