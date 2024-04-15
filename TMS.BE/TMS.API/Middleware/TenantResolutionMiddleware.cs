using System.Text.RegularExpressions;
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
                        var connectionString = await tenantService.GetTenantConnectionString(tenantId);
                        if (string.IsNullOrEmpty(connectionString))
                        {
                            throw new Exception("Can not resolve connection string for tenant: " + tenantId);
                        }
                        context.Items["ConnectionString"] = Regex.Unescape(connectionString);
                        
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
