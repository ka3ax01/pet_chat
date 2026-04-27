using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Infrastructure.Persistence.Configurations;

public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("MessageAttachments", DbSchemas.Messaging);
        builder.HasKey(ma => ma.Id);

        builder.Property(ma => ma.FileName).IsRequired().HasMaxLength(255);

        builder.Property(ma => ma.FileUrl).IsRequired().HasMaxLength(2048);

        builder.Property(ma => ma.ThumbnailUrl).HasMaxLength(2048);

        builder.Property(ma => ma.FileSizeBytes).IsRequired();

        builder.Property(ma => ma.IsDeleted).IsRequired();

        // Relationships
        builder
            .HasOne(ma => ma.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(ma => ma.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ma => ma.CreatedBy)
            .WithMany(cb => cb.CreatedMessageAttachments)
            .HasForeignKey(ma => ma.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
