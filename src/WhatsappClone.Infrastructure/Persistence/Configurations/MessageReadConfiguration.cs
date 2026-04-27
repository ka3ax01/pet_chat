using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Infrastructure.Persistence.Configurations;

public class MessageReadConfiguration : IEntityTypeConfiguration<MessageRead>
{
    public void Configure(EntityTypeBuilder<MessageRead> builder)
    {
        builder.ToTable("MessageReads", DbSchemas.Messaging);
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.MessageId, x.UserId }).IsUnique();

        builder
            .HasOne(x => x.Message)
            .WithMany()
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
