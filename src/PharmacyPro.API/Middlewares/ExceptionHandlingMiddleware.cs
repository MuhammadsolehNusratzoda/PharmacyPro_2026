using System.Text.Json;
using PharmacyPro.Application.Responses;

namespace PharmacyPro.API.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
        {
            // клиент request-ро бекор кард
        }
        catch (BadHttpRequestException ex)
        {
            await Write(ctx, Response<object>.Fail("Request нодуруст аст", 400,
                env.IsDevelopment() ? [ex.Message] : null));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
            await Write(ctx, Response<object>.Fail("Хатои дохилии сервер", 500,
                env.IsDevelopment() ? [ex.Message] : null));
        }
    }

    private static async Task Write(HttpContext ctx, Response<object> response)
    {
        if (ctx.Response.HasStarted) return;
        ctx.Response.Clear();
        ctx.Response.StatusCode = response.StatusCode;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(response, Json));
    }
}
