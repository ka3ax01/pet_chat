using WhatsappClone.Domain.Common;

namespace WhatsappClone.Domain.Entities;

public class Role : BaseEntity<int>
{
    public string Name { get; set; } = default!;

    #region Navigation Properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    #endregion
}
