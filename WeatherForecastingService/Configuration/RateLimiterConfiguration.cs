using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace WeatherForecastingService.Configuration;

public static class RateLimiterConfiguration
{
    public static string RateLimiterPolicyName => "FixedWindowRateLimiter";
    
    /// <summary>
    /// Configures a rate limiter.
    /// A maximum of 10 requests per each 10-second window are allowed
    /// </summary>
    public static void ConfigureRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.AddFixedWindowLimiter(policyName: RateLimiterPolicyName, options =>
            {
                options.PermitLimit = 10;
                options.Window = TimeSpan.FromSeconds(10);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 5;
                options.AutoReplenishment = true;
            });
        });
    }
}