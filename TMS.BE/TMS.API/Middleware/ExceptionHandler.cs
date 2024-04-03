using System.Text.Json;

namespace TMS.API.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }
        /// <summary>
        /// Hàm bắt sự kiện của middleware
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        /// Author: DNT(26/05/2023)
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Xử lý khi nhận được exception
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        /// Author: DNT(26/05/2023)
        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";
            var exceptionResponse = new
            {
                Error = true,
                Message = exception.Message,
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(exceptionResponse));
        }
    }
}
