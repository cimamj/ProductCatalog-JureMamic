using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ExternalServiceUnavailableException ex)
        {
            _logger.LogError(ex, "External service unavailable.");
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = ex.Message, code = "EXTERNAL_SERVICE_DOWN" });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed.");
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = "External service is unavailable.", code = "EXTERNAL_SERVICE_DOWN" });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timed out.");
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = "External service timed out.", code = "EXTERNAL_SERVICE_DOWN" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = "Internal server error." });
        }
    }
}