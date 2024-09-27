using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace WeatherForecastingService.Helpers;

public static class DistributedCacheExtensions
{
    public static async Task<T?> GetObject<T>(this IDistributedCache distributedCache, string key)
    {
        var cachedValueAsStr = await distributedCache.GetStringAsync(key);
        return cachedValueAsStr != null
            ? DeserializeObject<T>(cachedValueAsStr)
            : default;
    }
    
    public static async Task SetObject<T>(this IDistributedCache distributedCache,
        string key, T value, double absoluteExpirationInMinutes = 5)
    {
        if (value == null)
        {
            return;
        }
        
        var valueAsStr = SerializeObject(value);
        await distributedCache.SetStringAsync(key, valueAsStr, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteExpirationInMinutes)
        });
    }
    
    private static string SerializeObject(object value) => JsonSerializer.Serialize(value);

    private static TValue DeserializeObject<TValue>(string stringValue) => JsonSerializer.Deserialize<TValue>(stringValue)!;
}