using AspNetGlobalExceptionHandlerMiddleware;
using Microsoft.AspNetCore.Builder;

namespace RequestResponseLoggerMiddleware
{
    public static class CustomMiddlewareExtensions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app) 
            => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
