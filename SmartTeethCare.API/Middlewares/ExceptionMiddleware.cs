using SmartTeethCare.API.Errors;
using System.Net;
using System.Text.Json;

namespace SmartTeethCare.API.Middlewares
{
    // By Convention
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment environment;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
        {
            this.next = next;
            this.logger = logger;
            this.environment = environment;
        }
        public async Task InvokeAsync(HttpContext httpcontext)
        {
            try
            {
                await next.Invoke(httpcontext); // Call the next middleware in the pipeline
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message); // Log the exception details // Development
                // log exception details to a file or database // Production
                httpcontext.Response.ContentType = "application/json";
                httpcontext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = environment.IsDevelopment()
                    ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace?.ToString())
                    : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, "Internal Server Error");
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonResponse = JsonSerializer.Serialize(response, options);
                await httpcontext.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
