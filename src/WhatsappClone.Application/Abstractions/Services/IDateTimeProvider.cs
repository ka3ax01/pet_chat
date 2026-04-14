namespace WhatsappClone.Application.Abstractions.Services;

public interface IDateTimeProvider
{
    DateTimeOffset Now { get; }
}
