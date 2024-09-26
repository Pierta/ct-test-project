using System.Net;
using ApiClients;
using Polly;
using Polly.Extensions.Http;
using Refit;
using WeatherForecastingService.Configuration;
using WeatherForecastingService.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var buildInfoOptions = new BuildInfoOptions
{
    BranchName = Environment.GetEnvironmentVariable("GIT_BRANCH_NAME"),
    CommitHash = Environment.GetEnvironmentVariable("GIT_COMMIT_HASH")
};
builder.Services.ConfigureOpenTelemetry(buildInfoOptions);
builder.Services.AddHealthChecks();

var weatherApiUrl = Environment.GetEnvironmentVariable("WEATHER_API_URL")
                    ?? throw new SystemException("Weather API URL is required");
var weatherApiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY")
                    ?? throw new SystemException("Weather API KEY is required");
builder.Services
    .AddRefitClient<IWeatherApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(weatherApiUrl))
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(3))
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.ConfigureHealthChecks();

app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (BadHttpRequestException ex)
    {
        // Handle parameter binding errors
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new
        {
            Error = "Please, specify proper parameters",
            Details = ex.Message
        });
    }
});

app.MapGet("/weather", async (string city, DateTime date, IWeatherApi weatherApi) =>
    {
        var maxForecastDate = DateTime.Today.AddDays(1);
        if (date < new DateTime(2010, 1, 1).Date || date > maxForecastDate)
        {
            return Results.BadRequest($"Date must be on or after 2010-01-01 and less than or equal to {maxForecastDate:yyyy-MM-dd}");
        }
        
        var weatherInformationResponse = await weatherApi.GetWeatherData(weatherApiKey, city, date);
        return weatherInformationResponse.StatusCode == HttpStatusCode.BadRequest
            ? Results.NotFound($"The city '{city}' could not be found") 
            : Results.Ok(weatherInformationResponse.Content);
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/version", () => new
    {
        VersionHelper.AssemblyVersion,
        GitBranchName = buildInfoOptions.BranchName,
        GitCommitHash = buildInfoOptions.CommitHash
    })
    .WithName("GetVersion")
    .WithOpenApi();

app.Run();
