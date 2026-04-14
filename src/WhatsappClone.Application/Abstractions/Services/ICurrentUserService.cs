namespace WhatsappClone.Application.Abstractions.Services;

public interface ICurrentUserService
{
    public string UserName { get; }

    public Guid UserId { get; }
}
