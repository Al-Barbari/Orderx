using Microsoft.AspNetCore.Http;

namespace SharedLibrary.Middleware
{
    public class GatewayGuard(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract Guard header from request
            var guard = context.Request.Headers["Api-Gateway-Guard"];

            // NULL means, request is not coming from gateway
            if(guard.FirstOrDefault() == null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service is Unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
        
    }
}
