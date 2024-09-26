namespace WeatherForecastingService.Helpers;

public static class VersionHelper
{
    public static string AssemblyVersion = typeof(VersionHelper).Assembly
        .GetName()
        .Version!
        .ToString();
}