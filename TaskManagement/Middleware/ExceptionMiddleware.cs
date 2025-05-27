using System.Net;
using System.Text.Json;
using TaskManagement.Application.Exceptions;

namespace TaskManagement.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = "";

            switch (exception)
            {
                case NotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case ArgumentException:
                    code = HttpStatusCode.BadRequest;
                    break;
                    // tambahkan jenis error lain sesuai kebutuhan
            }

            var response = new
            {
                status = (int)code,
                error = exception.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            result = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(result);
        }
    }
}
