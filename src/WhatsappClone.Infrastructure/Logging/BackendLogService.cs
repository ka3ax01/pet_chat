using WhatsappClone.Application.Abstractions.Logging;
using WhatsappClone.Application.Abstractions.Services;
using WhatsappClone.Domain.Entities;
using WhatsappClone.Infrastructure.Persistence;

namespace WhatsappClone.Infrastructure.Logging;

public class BackendLogService(AppDbContext dbContext, IDateTimeProvider dateTimeProvider) : IBackendLogService
{
    public async Task WriteAsync(BackendLogWriteRequest request, CancellationToken cancellationToken = default)
    {
        dbContext.BackendLogs.Add(
            new BackendLog
            {
                Id = Guid.NewGuid(),
                Timestamp = dateTimeProvider.Now,
                Level = request.Level,
                Message = request.Message,
                Exception = request.Exception,
                Source = request.Source,
                RequestPath = request.RequestPath,
                HttpMethod = request.HttpMethod,
                StatusCode = request.StatusCode,
                ElapsedMilliseconds = request.ElapsedMilliseconds,
                UserId = request.UserId,
                UserName = request.UserName,
                TraceId = request.TraceId,
                PropertiesJson = request.PropertiesJson
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
