using System.Net;
using System.Reflection.Metadata.Ecma335;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Entities.ErrorModel;

namespace CompanyEmployees.Extensions
{
    public static class ExceptionMiddleWareExtensions
    {
        
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {
            // In the code below, we’ve created an extension method in which 
            // we’ve registered the UseExceptionHandler middleware.
            // Then, we’ve populated the status code and the content type 
            // of our response, logged the error message, 
            // and finally returned the response with the custom created object.
            app.UseExceptionHandler(appError => 
            
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong :{contextFeature.Error} ");
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());
                    }
                });
            });
        }
    }
}