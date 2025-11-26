using System.Diagnostics;
using Serilog.Context;

namespace Microsoft.eShopWeb.Web.Middleware;

public class PerformanceLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceLoggingMiddleware> _logger;

    public PerformanceLoggingMiddleware(RequestDelegate next, ILogger<PerformanceLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        using (LogContext.PushProperty("ClientIp", ipAddress ?? "unknown"))
        {
            var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var responseTime = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ResponseTime}ms",
                    requestMethod,
                    requestPath,
                    context.Response.StatusCode,
                    responseTime);

                // Log slow requests as warnings
                if (responseTime > 1000)
                {
                    _logger.LogWarning("Slow request detected: {RequestMethod} {RequestPath} took {ResponseTime}ms",
                        requestMethod,
                        requestPath,
                        responseTime);
                }
            }
        }
    }
}
