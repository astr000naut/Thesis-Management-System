using System.Text.RegularExpressions;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;

namespace TMS.Worker.API.Middleware
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

                    var tenantInfo = await tenantService.GetTenantByIdAsync(tenantId);

                    if (tenantInfo != null)
                    {
                        context.Items["ConnectionString"] = Regex.Unescape(tenantInfo.DBConnection + "Database=" + tenantInfo.DBName);
                        context.Items["TenantId"] = tenantInfo.TenantId;
                        context.Items["MinioInfo"] = new MinioConnectionInfo
                        {
                            EndPoint = tenantInfo.MinioEndpoint,
                            AccessKey = tenantInfo.MinioAccessKey,
                            SecretKey = tenantInfo.MinioSecretKey,
                            BucketName = tenantInfo.MinioBucketName
                        };
                    }



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
