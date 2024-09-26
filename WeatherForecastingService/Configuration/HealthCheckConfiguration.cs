using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace WeatherForecastingService.Configuration;

public static class HealthCheckConfiguration
{
    private const string LivenessHealthCheckName = "Liveness";
    private const string ReadinessHealthCheckName = "Readiness";
    private const string StartupHealthCheckName = "Startup";
    
    public static void ConfigureHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks($"/HealthCheck/{LivenessHealthCheckName}", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                Predicate = healthCheck => healthCheck.Tags.Contains(LivenessHealthCheckName)
            })
            .AllowAnonymous();

        app.MapHealthChecks($"/HealthCheck/{ReadinessHealthCheckName}", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                Predicate = healthCheck => healthCheck.Tags.Contains(ReadinessHealthCheckName)
            })
            .AllowAnonymous();
        
        app.MapHealthChecks($"/HealthCheck/{StartupHealthCheckName}", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                Predicate = healthCheck => healthCheck.Tags.Contains(StartupHealthCheckName)
            })
            .AllowAnonymous();
    }
}