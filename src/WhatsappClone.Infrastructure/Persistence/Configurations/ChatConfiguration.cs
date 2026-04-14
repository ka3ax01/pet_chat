using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Infrastructure.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).HasMaxLength(255);

        builder
            .HasMany(c => c.Members)
            .WithOne(cm => cm.Chat)
            .HasForeignKey(cm => cm.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(c => c.CreatedBy)
            .WithMany(cb => cb.CreatedChats)
            .HasForeignKey(c => c.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
