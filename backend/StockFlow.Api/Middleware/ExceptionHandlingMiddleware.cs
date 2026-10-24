using System.Text.Json;
using Npgsql;

namespace StockFlow.Api.Middleware;

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
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
                throw;
            

            _logger.LogError(
                ex,
                "Error procesando la petición {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            var (statusCode, message) = ex switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "El recurso solicitado no existe."),

                ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),

                PostgresException pgEx
                    when pgEx.SqlState == "P0001" => (StatusCodes.Status400BadRequest, pgEx.MessageText), _ =>
                    (StatusCodes.Status500InternalServerError, "Ocurrió un error interno. Intenta nuevamente.")
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    statusCode,
                    message
                }));
        }
    }
}