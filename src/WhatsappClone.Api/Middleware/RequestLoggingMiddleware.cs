using System.Diagnostics;
using WhatsappClone.Application.Abstractions.Logging;
using WhatsappClone.Application.Abstractions.Services;

namespace WhatsappClone.Api.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IBackendLogService backendLogService, ICurrentUserService currentUserService)
    {
        var stopwatch = Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();

        if (context.Response.StatusCode < StatusCodes.Status400BadRequest && stopwatch.ElapsedMilliseconds < 1000)
        {
            return;
        }

        var level = context.Response.StatusCode >= StatusCodes.Status500InternalServerError ? "Error" : "Warning";
        var message = "HTTP request completed with notable status or latency.";

        logger.Log(
            context.Response.StatusCode >= StatusCodes.Status500InternalServerError ? LogLevel.Error : LogLevel.Warning,
            "{Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Request.Path.Value,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);

        try
        {
            await backendLogService.WriteAsync(
                new BackendLogWriteRequest(
                    Level: level,
                    Message: message,
                    Source: nameof(RequestLoggingMiddleware),
                    RequestPath: context.Request.Path.Value,
                    HttpMethod: context.Request.Method,
                    StatusCode: context.Response.StatusCode,
                    ElapsedMilliseconds: stopwatch.ElapsedMilliseconds,
                    UserId: currentUserService.UserId == Guid.Empty ? null : currentUserService.UserId,
                    UserName: string.IsNullOrWhiteSpace(currentUserService.UserName) ? null : currentUserService.UserName,
                    TraceId: context.TraceIdentifier),
                context.RequestAborted);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to persist backend request log.");
        }
    }
}
