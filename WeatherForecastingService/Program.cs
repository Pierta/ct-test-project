using System.Net;
using ApiClients;
using Asp.Versioning;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Extensions.Http;
using Refit;
using WeatherForecastingService.Configuration;
using WeatherForecastingService.Helpers;
using WeatherForecastingService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
if (!string.IsNullOrWhiteSpace(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(option =>
    {
        option.Configuration = redisConnectionString;
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

var buildInfoOptions = new BuildInfoOptions
{
    BranchName = Environment.GetEnvironmentVariable("GIT_BRANCH_NAME"),
    CommitHash = Environment.GetEnvironmentVariable("GIT_COMMIT_HASH")
};
builder.Services.ConfigureOpenTelemetry(buildInfoOptions);
builder.Services.AddHealthChecks();
builder.Services.ConfigureRateLimiter();
builder.Services.ConfigureApiVersioning();

var weatherApiUrl = Environment.GetEnvironmentVariable("WEATHER_API_URL")
                    ?? throw new SystemException("Weather API URL is required");
builder.Services
    .AddRefitClient<IWeatherApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(weatherApiUrl))
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
    );

var weatherApiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY")
                    ?? throw new SystemException("Weather API KEY is required");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.ConfigureHealthChecks();
app.UseRateLimiter();
app.UseMiddleware<ExceptionHandlingMiddleware>();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

app.MapGet("api/v{version:apiVersion}/weather", async (string city, DateTime date,
        IWeatherApi weatherApi, IDistributedCache distributedCache) =>
    {
        var maxForecastDate = DateTime.Today.AddDays(1);
        if (date < new DateTime(2010, 1, 1).Date || date > maxForecastDate)
        {
            return Results.BadRequest($"Date must be on or after 2010-01-01 and less than or equal to {maxForecastDate:yyyy-MM-dd}");
        }

        var cacheKey = $"{city}-{date:yyyy-MM-dd}";
        var cachedWeatherData = await distributedCache.GetObject<WeatherDataVm>(cacheKey);
        if (cachedWeatherData != null)
        {
            return Results.Ok(cachedWeatherData);
        }
        
        var weatherInformationResponse = await weatherApi.GetWeatherData(weatherApiKey, city, date);
        if (weatherInformationResponse.StatusCode == HttpStatusCode.BadRequest)
        {
            return Results.NotFound($"The city '{city}' could not be found");
        }

        var result = WeatherDataMapper.ToVm(weatherInformationResponse.Content);
        await distributedCache.SetObject(cacheKey, result);
        return Results.Ok(result);
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi()
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1)
    .RequireRateLimiting(RateLimiterConfiguration.RateLimiterPolicyName);

app.MapGet("api/v{version:apiVersion}/version", () => new
    {
        VersionHelper.AssemblyVersion,
        GitBranchName = buildInfoOptions.BranchName,
        GitCommitHash = buildInfoOptions.CommitHash
    })
    .WithName("GetVersion")
    .WithOpenApi()
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1);

app.Run();
