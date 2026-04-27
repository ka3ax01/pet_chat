using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsappClone.Domain.Constants;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", DbSchemas.Auth);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasData(
            new Role { Id = SystemRoles.AdminId, Name = SystemRoles.Admin },
            new Role { Id = SystemRoles.UserId, Name = SystemRoles.User });
    }
}
