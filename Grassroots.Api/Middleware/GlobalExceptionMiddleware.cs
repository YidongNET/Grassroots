using System.Net;
using System.Text.Json;
using Grassroots.Api.Models;

namespace Grassroots.Api.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            UnauthorizedAccessException => ApiResponse<object>.Error(
                (int)HttpStatusCode.Unauthorized,
                "Unauthorized access"),
                
            ArgumentException => ApiResponse<object>.Error(
                (int)HttpStatusCode.BadRequest,
                exception.Message),
                
            KeyNotFoundException => ApiResponse<object>.Error(
                (int)HttpStatusCode.NotFound,
                exception.Message),
                
            InvalidOperationException => ApiResponse<object>.Error(
                (int)HttpStatusCode.BadRequest,
                exception.Message),
                
            _ => ApiResponse<object>.Error(
                (int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred")
        };

        context.Response.StatusCode = response.Code;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
} 