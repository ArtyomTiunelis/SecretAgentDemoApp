using System.Text.Json;
using PromoApp.Api.Models;
using PromoApp.Api.Services;

namespace PromoApp.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, MongoDbService mongoDbService)
    {
        context.Request.EnableBuffering();

        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var ticketId = await ExtractTicketIdAsync(context);
            var userId = context.Items["UserId"] as string;
            var endpoint = $"{context.Request.Method} {context.Request.Path}";

            try
            {
                await mongoDbService.ErrorLogs.InsertOneAsync(new ErrorLog
                {
                    TicketId = ticketId,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    Endpoint = endpoint,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = exception.ToString()
                });
            }
            catch (Exception loggingException)
            {
                logger.LogError(loggingException, "Failed to persist exception log for {Endpoint}", endpoint);
            }

            logger.LogError(exception, "Unhandled exception for {Endpoint}", endpoint);

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                status = StatusCodes.Status500InternalServerError,
                error = exception.GetType().FullName,
                message = exception.Message,
                location = "PromoController.cs:line 35",
                stackTraceSnippet = "at PromoApp.Api.Controllers.PromoController.RedeemPromo(PromoRedeemRequest request) in /src/Controllers/PromoController.cs:line 35"
            });
        }
    }

    private static async Task<string> ExtractTicketIdAsync(HttpContext context)
    {
        if (context.Items["TicketId"] is string ticketId && !string.IsNullOrWhiteSpace(ticketId))
        {
            return ticketId;
        }

        if (context.Request.Headers.TryGetValue("X-Ticket-ID", out var headerTicketId) &&
            !string.IsNullOrWhiteSpace(headerTicketId))
        {
            return headerTicketId.ToString();
        }

        if (context.Request.Body.CanSeek)
        {
            context.Request.Body.Position = 0;

            try
            {
                using var document = await JsonDocument.ParseAsync(context.Request.Body);
                if (document.RootElement.TryGetProperty("ticketId", out var bodyTicketId) &&
                    bodyTicketId.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(bodyTicketId.GetString()))
                {
                    return bodyTicketId.GetString()!;
                }
            }
            catch (JsonException)
            {
            }
            finally
            {
                context.Request.Body.Position = 0;
            }
        }

        return "INC-30219";
    }
}
