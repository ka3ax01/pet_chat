using WhatsappClone.Domain.Common;

namespace WhatsappClone.Domain.Entities;

public class BackendLog : BaseEntity<Guid>
{
    public DateTimeOffset Timestamp { get; set; }
    public string Level { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? Exception { get; set; }
    public string? Source { get; set; }
    public string? RequestPath { get; set; }
    public string? HttpMethod { get; set; }
    public int? StatusCode { get; set; }
    public long? ElapsedMilliseconds { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? TraceId { get; set; }
    public string? PropertiesJson { get; set; }
}
