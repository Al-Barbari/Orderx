using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare Variable
            string messgae = "Sorry, Internal Server Error occured";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Erorr";

            try
            {
                await next(context);

                // Check if the Response is Too Many Requests // 429 status code
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    statusCode = (int) HttpStatusCode.TooManyRequests;
                    messgae = "Too Many Requests";
                    title = "Warning";

                    await ModifyHeader(context, statusCode, messgae, title);
                }

                // Check if the Response is Unauthorized // 401 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    messgae = "You are not authorized to access";
                    title = "Alert";
                    await ModifyHeader(context, statusCode, messgae, title);
                }

                // Check if the Response is Forbidden // 403 status code
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    statusCode = (int)HttpStatusCode.Forbidden;
                    messgae = "You are not allowed to access";
                    title = "Out Of Access";
                    await ModifyHeader(context, statusCode, messgae, title);
                }
            }
            catch (Exception ex)
            {
                // in Production we should log the exception to the database, file or any other logging service
                // Log orginal execption / File, Dubugger, Console
                await ModifyHeader(context, statusCode, messgae, title);

                // Check if the exception is timeout exception
                
                
            }
        }

        private static async Task ModifyHeader(HttpContext context, int statusCode, string messgae, string title)
        {
            // display scary-free message to client 
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Title = title,
                Status = statusCode,
                Detail = messgae
            }),CancellationToken.None);
            return;
        }
    }
}
