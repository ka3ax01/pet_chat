using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Infrastructure.Persistence.Configurations;

public class BackendLogConfiguration : IEntityTypeConfiguration<BackendLog>
{
    public void Configure(EntityTypeBuilder<BackendLog> builder)
    {
        builder.ToTable("BackendLogs", DbSchemas.Audit);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Level).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Exception).HasColumnType("text");
        builder.Property(x => x.Source).HasMaxLength(255);
        builder.Property(x => x.RequestPath).HasMaxLength(2048);
        builder.Property(x => x.HttpMethod).HasMaxLength(20);
        builder.Property(x => x.UserName).HasMaxLength(255);
        builder.Property(x => x.TraceId).HasMaxLength(255);
        builder.Property(x => x.PropertiesJson).HasColumnType("jsonb");

        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => x.Level);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TraceId);
    }
}
