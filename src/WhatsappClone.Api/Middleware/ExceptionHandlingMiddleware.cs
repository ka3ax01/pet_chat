using System.Net;
using WhatsappClone.Application.Abstractions.Logging;
using WhatsappClone.Application.Abstractions.Services;

namespace WhatsappClone.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IBackendLogService backendLogService, ICurrentUserService currentUserService)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path.Value);

            await WriteExceptionLogAsync(context, backendLogService, currentUserService, exception);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(
                new
                {
                    error = "An unexpected error occurred.",
                    traceId = context.TraceIdentifier
                });
        }
    }

    private static async Task WriteExceptionLogAsync(
        HttpContext context,
        IBackendLogService backendLogService,
        ICurrentUserService currentUserService,
        Exception exception)
    {
        try
        {
            await backendLogService.WriteAsync(
                new BackendLogWriteRequest(
                    Level: "Error",
                    Message: exception.Message,
                    Exception: exception.ToString(),
                    Source: nameof(ExceptionHandlingMiddleware),
                    RequestPath: context.Request.Path.Value,
                    HttpMethod: context.Request.Method,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    UserId: currentUserService.UserId == Guid.Empty ? null : currentUserService.UserId,
                    UserName: string.IsNullOrWhiteSpace(currentUserService.UserName) ? null : currentUserService.UserName,
                    TraceId: context.TraceIdentifier),
                context.RequestAborted);
        }
        catch
        {
            // Logging must not hide the original exception response.
        }
    }
}
