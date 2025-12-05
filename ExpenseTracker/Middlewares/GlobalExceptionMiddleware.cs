using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpenseTracker.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException badRequestEx)
        {
            Log.Error(badRequestEx, "Bad Request");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Type = null,
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = badRequestEx.Message.Replace("\"", "'"),
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Type = null,
                Title = "InternalServerError",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
            });
        }
    }
}