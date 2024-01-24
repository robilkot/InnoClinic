﻿using Microsoft.AspNetCore.Mvc;
using OfficesService.Exceptions;
using Serilog;

namespace OfficesService.Middleware
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
            catch (Exception ex)
            {
                if (ex is OfficesException officesEx)
                {
                    context.Response.StatusCode = (int)officesEx.StatusCode!;
                    Log.Error("OfficesException caught: {msg}", ex.Message);

                    if (officesEx.StatusCode != null)
                    { 
                        context.Response.StatusCode = (int)officesEx.StatusCode;
                    }
                }
                else
                {
                    Log.Error("Exception caught: {msg}", ex.Message);
                    await context.Response.WriteAsync($"Exception caught not from program-specific layer: {ex.Message}");
                }
            }
        }
    }
    public static class ExceptionHandlingMiddlewareExtension
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
