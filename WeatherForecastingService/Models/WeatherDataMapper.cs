using System.Diagnostics.CodeAnalysis;
using ApiClients;
using Riok.Mapperly.Abstractions;

namespace WeatherForecastingService.Models;

[Mapper]
public static partial class WeatherDataMapper
{
    public static WeatherDataVm? ToVm(WeatherDataResponse? weatherDataResponse)
    {
        if (weatherDataResponse == null)
            return default;
        
        var target = MapToWeatherDataVm(weatherDataResponse);
        target.WeatherPerDay = [];
        foreach (var weatherData in weatherDataResponse.Forecast.ForecastDay)
        {
            var weatherPerDay = MapToWeatherPerDay(weatherData.Day);
            weatherPerDay.Date = weatherData.Date;
            target.WeatherPerDay.Add(weatherPerDay);
        }
        
        return target;
    }
    
    [return: NotNullIfNotNull(nameof(weatherDataResponse))]
    [MapNestedProperties(nameof(WeatherDataResponse.Location))]
    [MapProperty([nameof(WeatherDataResponse.Location), nameof(WeatherDataResponse.Location.Name)], [nameof(WeatherDataVm.City)])]
    private static partial WeatherDataVm? MapToWeatherDataVm(WeatherDataResponse? weatherDataResponse);
    
    [MapProperty([nameof(DayData.Condition), nameof(DayData.Condition.Text)], [nameof(WeatherPerDay.WeatherCondition)])]
    private static partial WeatherPerDay MapToWeatherPerDay(DayData dayData);
}