namespace WhatsappClone.Application.Abstractions.Logging;

public interface IBackendLogService
{
    Task WriteAsync(BackendLogWriteRequest request, CancellationToken cancellationToken = default);
}

public sealed record BackendLogWriteRequest(
    string Level,
    string Message,
    string? Exception = null,
    string? Source = null,
    string? RequestPath = null,
    string? HttpMethod = null,
    int? StatusCode = null,
    long? ElapsedMilliseconds = null,
    Guid? UserId = null,
    string? UserName = null,
    string? TraceId = null,
    string? PropertiesJson = null);
