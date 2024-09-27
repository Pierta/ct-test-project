namespace WeatherForecastingService.Models;

public class WeatherDataVm
{
    public string? City { get; set; }
        
    public string? Region { get; set; }
        
    public string? Country { get; set; }
    
    public string? TimeZone { get; set; }
        
    public string? Localtime { get; set; }
    
    public List<WeatherPerDay>? WeatherPerDay { get; set; }
}

public class WeatherPerDay
{
    public string? Date { get; set; }
    
    public double Temperature { get; set; }
    
    public double Wind { get; set; }
    
    public int Humidity { get; set; }
    
    public double Visibility { get; set; }
        
    public double Uv { get; set; }
    
    public string? WeatherCondition { get; set; }
}