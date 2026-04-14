namespace WhatsappClone.Domain.Enums;

/// <summary>
/// Defines the role of a chat member within a chat. This can be used to determine permissions and access levels for different members in a chat group.
/// </summary>
/// <remarks>
/// - Member: A regular member of the chat with standard permissions.
/// - Author: A member who can create and edit messages in the chat.
/// - Admin: A member with administrative privileges, including the ability to manage other members and settings.
/// </remarks>
public enum ChatMemberRole
{
    Member = 0,
    Author = 1,
    Admin = 2,
}
