using ArtGalleryAPI.DTOs;
using System.Net;
using System.Text.Json;

namespace ArtGalleryAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            int statusCode;
            string message;
            List<string>? errors = null;

            switch (exception)
            {
                case ArgumentException argEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = argEx.Message;
                    break;
                case InvalidOperationException invEx:
                    statusCode = (int)HttpStatusCode.Conflict;
                    message = invEx.Message;
                    break;
                case KeyNotFoundException keyEx:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = keyEx.Message;
                    break;
                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "Unauthorized access";
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred";
                    errors = new List<string> { exception.Message };
                    break;
            }

            response.StatusCode = statusCode;

            var errorResponse = ApiResponse.ErrorResponse(message, errors);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
        }
    }
}
