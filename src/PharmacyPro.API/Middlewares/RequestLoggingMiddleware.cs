using System.Diagnostics;

namespace PharmacyPro.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        var sw = Stopwatch.StartNew();
        await next(ctx);
        sw.Stop();

        var level = sw.ElapsedMilliseconds > 1000 ? LogLevel.Warning : LogLevel.Information;
        logger.Log(level, "{Method} {Path} -> {Status} ({Ms} ms)",
            ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
