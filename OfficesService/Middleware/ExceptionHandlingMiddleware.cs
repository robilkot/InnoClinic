using CommonData.Exceptions;
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
                if (ex is InnoClinicException clinicEx)
                {
                    context.Response.StatusCode = (int)clinicEx.StatusCode!;
                    Log.Error("InnoClinicException caught: {msg}", ex.Message);

                    if (clinicEx.StatusCode != null)
                    {
                        context.Response.StatusCode = (int)clinicEx.StatusCode;
                    }
                }
                else
                {
                    Log.Error("Exception caught: {msg}", ex.Message);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    //await context.Response.WriteAsync($"Exception caught not from program-specific layer: {ex.Message}");
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
