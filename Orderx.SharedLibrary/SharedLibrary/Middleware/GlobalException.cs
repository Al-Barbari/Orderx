using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.logs;
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

                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status429TooManyRequests:
                        statusCode = (int)StatusCodes.Status429TooManyRequests;
                        messgae = "Too Many Requests";
                        title = "Warning";
                        break;
                    case StatusCodes.Status401Unauthorized:
                        statusCode = (int)StatusCodes.Status401Unauthorized;
                        messgae = "You are not authorized to access";
                        title = "Alert";
                        break;
                    case StatusCodes.Status403Forbidden:
                        statusCode = (int)StatusCodes.Status403Forbidden;
                        messgae = "You are not allowed to access";
                        title = "Out Of Access";
                        break;
                }
            }
            catch (Exception ex)
            {
                // in Production we should log the exception to the database, file or any other logging service
                // Log orginal execption / File, Dubugger, Console
                LogExcepion.LogExceptions(ex);

                // Check if the exception is timeout exception
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    statusCode = (int)StatusCodes.Status408RequestTimeout;
                    messgae = "Request Timeout";
                    title = "Timeout";
                }
            }
            finally
            {
                await ModifyHeader(context, statusCode, messgae, title);
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
