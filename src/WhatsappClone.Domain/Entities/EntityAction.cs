using WhatsappClone.Domain.Common;
using WhatsappClone.Domain.Enums;

namespace WhatsappClone.Domain.Entities;

public class EntityAction : BaseEntity<Guid>
{
    public string EntityName { get; set; } = default!;
    public string EntityId { get; set; } = default!;
    public EntityActionType ActionType { get; set; }
    public string? ChangesJson { get; set; }
    public DateTimeOffset PerformedAt { get; set; }
    public Guid? PerformedById { get; set; }

    public User? PerformedBy { get; set; }
}
