using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Refit;

namespace ApiClients
{
    public interface IWeatherApi
    {
        [Get("/history.json?key={apiKey}&q={city}&dt={dateTime}")]
        Task<ApiResponse<WeatherDataResponse>> GetWeatherData(string apiKey, string city, DateTime dateTime);
    }
    
    public class WeatherDataResponse
    {
        public LocationData Location { get; set; }
        
        public ForecastData Forecast { get; set; }
    }

    public class ForecastData
    {
        public List<WeatherData> ForecastDay { get; set; }
    }

    public class LocationData
    {
        public string Name { get; set; }
        
        public string Region { get; set; }
        
        public string Country { get; set; }
        
        [JsonPropertyName("tz_id")]
        public string TimeZone { get; set; }
        
        public string Localtime { get; set; }
    }

    public class WeatherData
    {
        public DayData Day { get; set; }
    }

    public class DayData
    {
        [JsonPropertyName("avgtemp_c")]
        public double Temperature { get; set; }

        [JsonPropertyName("maxwind_kph")]
        public double Wind { get; set; }
        
        [JsonPropertyName("avghumidity")]
        public int Humidity { get; set; }
        
        [JsonPropertyName("avgvis_km")]
        public double Visibility { get; set; }
        
        public WeatherCondition Condition { get; set; }
        
        public double Uv { get; set; }
    }

    public class WeatherCondition
    {
        public string Text { get; set; }
    }
}