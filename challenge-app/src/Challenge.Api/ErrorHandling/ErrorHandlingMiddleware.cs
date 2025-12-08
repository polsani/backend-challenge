namespace Challenge.Api.ErrorHandling;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception occurred: {Message}", e.Message);
            await HandleExceptionAsync(context, e);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ApplicationException _ => new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = exception.Message,
                Details = exception.InnerException?.Message
            },
            KeyNotFoundException _ => new ErrorResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Resource not found",
                Details = exception.Message
            },
            UnauthorizedAccessException _ => new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Unauthorized access",
                Details = exception.Message
            },
            ArgumentException _ => new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid argument",
                Details = exception.Message
            },
            _ => new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An internal server error occurred",
                Details = exception.Message
            }
        };

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}