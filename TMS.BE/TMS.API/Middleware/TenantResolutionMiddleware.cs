using System.Text.Json;
using System.Text.RegularExpressions;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;

namespace TMS.API.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            try
            {
                if (!context.Items.ContainsKey("ConnectionString"))
                {
                    var tenantId = context.Request.Cookies["x-tenantid"];
                    if (tenantId != null)
                    {       
                        var tenantInfo = await tenantService.GetTenantByIdAsync(tenantId);

                        if (tenantInfo == null || tenantInfo.Status != 2)
                        {
                            // response serviceresponse object
                            context.Response.ContentType = "application/json";
                            var exceptionResponse = new
                            {
                                success = false,
                                errorCode = "TENANT_NOT_ACTIVE",
                                message = "Hệ thống chưa được kích hoạt để sử dụng. Vui lòng liên hệ quản trị viên."
                            };
                            await context.Response.WriteAsync(JsonSerializer.Serialize(exceptionResponse));
                            return;

                        }

                        if (tenantInfo != null)
                        { 
                            context.Items["ConnectionString"] = Regex.Unescape(tenantInfo.DBConnection + "Database=" + tenantInfo.DBName);
                            context.Items["TenantId"] = tenantId;
                        }


                 
                    }
                    
                }

                // Get x-token from cookies and set to header
                var accessToken = context.Request.Cookies["x-token"];

                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + accessToken);
                }

                await _next(context);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
