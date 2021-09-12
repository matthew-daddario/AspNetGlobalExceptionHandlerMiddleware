using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AspNetGlobalExceptionHandlerMiddleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _jsonContentType = "application/json";

        public GlobalExceptionHandlerMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BadHttpRequestException e) // Kestrel exception
            {
                Log.Write(LogEventLevel.Error, e.Message);

                await WriteErrorResponse(httpContext, e.StatusCode, e.Message);
            }
            catch (Exception e)
            {
                // Add logic (e.g. switch case/expression) to determine HTTP response status code
                var responseStatusCode = StatusCodes.Status500InternalServerError;

                Log.Error(e, e.Message);

                await WriteErrorResponse(httpContext, responseStatusCode, e.Message);
            }
        }

        private static Task WriteErrorResponse(HttpContext context, int statusCode, string errorMessage)
        {
            SetResponseStatusCode(context, statusCode);
            return context.Response.WriteAsync(JsonSerializer.Serialize<object>(new { errorMessage = errorMessage }));
        }

        private static void SetResponseStatusCode(HttpContext context, int statusCode)
        {
            context.Response.ContentType = _jsonContentType;
            context.Response.StatusCode = statusCode;
        }
    }
}
