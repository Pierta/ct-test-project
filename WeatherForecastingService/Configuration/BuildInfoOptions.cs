namespace WeatherForecastingService.Configuration;

public class BuildInfoOptions
{
    public string? BranchName { get; init; }

    public string? CommitHash { get; init; }
}