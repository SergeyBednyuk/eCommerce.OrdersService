namespace eCommerce.OrdersService.API.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            //TODO: log the exception type and message
            _logger.LogError($"{ex.GetType().ToString()}: {ex.Message}");
            Console.WriteLine(ex.Message);

            if (ex.InnerException is not null)
            {
                _logger.LogError($"{ex.InnerException.GetType().ToString()}: {ex.InnerException.Message}");
                Console.WriteLine(ex.InnerException.Message);
            }
            
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new {Message = ex.Message, Exception = ex.GetType().ToString()});
        }
    }
}