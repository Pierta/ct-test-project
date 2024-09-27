namespace WeatherForecastingService.Configuration;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
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
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
            
            // Handle everything else
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "An unexpected error occurred. Please try again later."
            });
        }
    }
}