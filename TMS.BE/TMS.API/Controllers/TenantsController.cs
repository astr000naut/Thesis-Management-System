using Microsoft.AspNetCore.Mvc;
using TMS.API.Param;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost("base-info")]
        public async Task<IActionResult> GetTenantBaseInfo([FromBody] TenantInfoParam param)
        {
            TenantLiteDto tenantLiteDto = await _tenantService.GetTenantBaseInfo(param.Domain);
            if (tenantLiteDto == null)
            {
                return NotFound();
            }

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.MaxValue,
                Secure = false, 
                HttpOnly = false,
                SameSite = SameSiteMode.Lax
            };

            HttpContext.Response.Cookies.Append("x-tenantid", tenantLiteDto.TenantId.ToString() ?? "", cookieOptions);
            return Ok(tenantLiteDto);
        }


    }
}
