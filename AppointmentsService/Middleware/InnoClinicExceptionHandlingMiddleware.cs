using CommonData.Exceptions;
using Serilog;

namespace AppointmentsService.Middleware
{

    public class InnoClinicExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public InnoClinicExceptionHandlingMiddleware(RequestDelegate next)
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
                }
            }
        }
    }
    public static class ExceptionHandlingMiddlewareExtension
    {
        public static IApplicationBuilder UseInnoClinicExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<InnoClinicExceptionHandlingMiddleware>();
        }
    }
}
