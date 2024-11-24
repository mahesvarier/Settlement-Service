using System.Net;
using System.Text.Json;

namespace Settlement_Service.Utilities
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadHttpRequestException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, BadHttpRequestException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new
            {
                ErrorCode = "INVALID_REQUEST_BODY",
                Message = "The request body is not a valid JSON."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
