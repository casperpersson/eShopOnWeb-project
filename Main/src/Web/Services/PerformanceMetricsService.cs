using System.Diagnostics;

namespace Microsoft.eShopWeb.Web.Services;

public interface IPerformanceMetricsService
{
    void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime, TimeSpan duration, bool success);
    void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success);
}

public class PerformanceMetricsService : IPerformanceMetricsService
{
    private readonly ILogger<PerformanceMetricsService> _logger;

    public PerformanceMetricsService(ILogger<PerformanceMetricsService> logger)
    {
        _logger = logger;
    }

    public void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime, TimeSpan duration, bool success)
    {
        _logger.LogInformation("Dependency {DependencyName} {CommandName} executed in {Duration}ms, Success: {Success}",
            dependencyName, commandName, duration.TotalMilliseconds, success);
    }

    public void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success)
    {
        _logger.LogInformation("Request {RequestName} completed in {Duration}ms with response {ResponseCode}, Success: {Success}",
            name, duration.TotalMilliseconds, responseCode, success);
    }
}
