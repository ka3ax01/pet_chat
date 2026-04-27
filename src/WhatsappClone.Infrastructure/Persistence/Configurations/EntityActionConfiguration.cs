using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Infrastructure.Persistence.Configurations;

public class EntityActionConfiguration : IEntityTypeConfiguration<EntityAction>
{
    public void Configure(EntityTypeBuilder<EntityAction> builder)
    {
        builder.ToTable("EntityActions", DbSchemas.Audit);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.EntityId).IsRequired().HasMaxLength(255);
        builder.Property(x => x.ChangesJson).HasColumnType("jsonb");

        builder
            .HasOne(x => x.PerformedBy)
            .WithMany(x => x.EntityActions)
            .HasForeignKey(x => x.PerformedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
