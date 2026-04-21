using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatsappClone.Infrastructure.Persistence;

namespace WhatsappClone.Api.Controllers;

[ApiController]
[Route("api/admin/logs")]
public class AdminLogsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AdminLogPageResponse>> GetLogs(
        [FromQuery] string? level,
        [FromQuery] string? search,
        [FromQuery] string? path,
        [FromQuery] Guid? userId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var query = dbContext.BackendLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(level))
        {
            query = query.Where(x => x.Level == level);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Message.Contains(search) ||
                (x.Exception != null && x.Exception.Contains(search)) ||
                (x.TraceId != null && x.TraceId.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(path))
        {
            query = query.Where(x => x.RequestPath != null && x.RequestPath.Contains(path));
        }

        if (userId is not null)
        {
            query = query.Where(x => x.UserId == userId);
        }

        if (from is not null)
        {
            query = query.Where(x => x.Timestamp >= from);
        }

        if (to is not null)
        {
            query = query.Where(x => x.Timestamp <= to);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminLogItemResponse(
                x.Id,
                x.Timestamp,
                x.Level,
                x.Message,
                x.Source,
                x.RequestPath,
                x.HttpMethod,
                x.StatusCode,
                x.ElapsedMilliseconds,
                x.UserId,
                x.UserName,
                x.TraceId,
                x.Exception,
                x.PropertiesJson))
            .ToListAsync(cancellationToken);

        return Ok(new AdminLogPageResponse(items, page, pageSize, total));
    }
}

public sealed record AdminLogPageResponse(
    IReadOnlyCollection<AdminLogItemResponse> Items,
    int Page,
    int PageSize,
    int Total);

public sealed record AdminLogItemResponse(
    Guid Id,
    DateTimeOffset Timestamp,
    string Level,
    string Message,
    string? Source,
    string? RequestPath,
    string? HttpMethod,
    int? StatusCode,
    long? ElapsedMilliseconds,
    Guid? UserId,
    string? UserName,
    string? TraceId,
    string? Exception,
    string? PropertiesJson);
