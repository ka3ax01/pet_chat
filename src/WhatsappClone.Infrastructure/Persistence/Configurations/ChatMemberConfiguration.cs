using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsappClone.Domain.Entities;

namespace WhatsappClone.Infrastructure.Persistence.Configurations;

public class ChatMemberConfiguration : IEntityTypeConfiguration<ChatMember>
{
    public void Configure(EntityTypeBuilder<ChatMember> builder)
    {
        builder.ToTable("ChatMembers", DbSchemas.Messaging);
        builder.HasKey(cm => new { cm.ChatId, cm.UserId });

        builder.HasOne(cm => cm.User).WithMany(u => u.ChatMembers).HasForeignKey(cm => cm.UserId);

        builder.HasOne(cm => cm.Chat).WithMany(c => c.ChatMembers).HasForeignKey(cm => cm.ChatId);
    }
}
