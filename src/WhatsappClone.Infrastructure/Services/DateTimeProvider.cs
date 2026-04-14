using WhatsappClone.Application.Abstractions.Services;

namespace WhatsappClone.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now.ToLocalTime();
}
