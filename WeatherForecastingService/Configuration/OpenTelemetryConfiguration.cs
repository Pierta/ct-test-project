using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WeatherForecastingService.Helpers;

namespace WeatherForecastingService.Configuration;

public static class OpenTelemetryConfiguration
{
    private static string ServiceName => nameof(WeatherForecastingService);
    
    private static IEnumerable<string> BlacklistedRequests { get; } =
    [
        "/api/v1/healthcheck/",
        "/hangfire",
        "/swagger/"
    ];

    /// <summary>
    /// Configures OpenTelemetry for logging, distributed tracing, and metrics.
    /// </summary>
    public static void ConfigureOpenTelemetry(this IServiceCollection services, BuildInfoOptions buildInfoOptions)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(builder =>
            {
                builder.Clear();
                builder.AddService(serviceName: ServiceName, serviceInstanceId: $"{ServiceName}-{Guid.NewGuid().ToString()}");
                builder.AddEnvironmentVariableDetector();
            })
            .WithTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = context =>
                        {
                            return !BlacklistedRequests.Any(r => context.Request.Path
                                .ToString()
                                .StartsWith(r, StringComparison.OrdinalIgnoreCase));
                        };
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag(nameof(VersionHelper.AssemblyVersion), VersionHelper.AssemblyVersion);
                            activity.SetTag(nameof(buildInfoOptions.BranchName), buildInfoOptions.BranchName);
                            activity.SetTag(nameof(buildInfoOptions.CommitHash), buildInfoOptions.CommitHash);
                        };
                    });

                builder.AddConsoleExporter();
            })
            .WithMetrics(builder =>
            {
                builder.AddRuntimeInstrumentation();
            })
            .UseOtlpExporter();
    }
}