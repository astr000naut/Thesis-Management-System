using Microsoft.AspNetCore.Mvc;
using TenantManagement.API.Param;
using TenantManagement.BusinessLayer.DTO;
using TenantManagement.BusinessLayer.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TenantManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : BaseController<TenantDto>
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService) : base(tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost("check-db-connection")]
        public async Task<IActionResult> CheckDBConnection([FromBody] CheckDBConnectionParam param)
        {
            var response = await _tenantService.CheckDBConnection(param.AutoCreateDB, param.ConnectionString, param.DBName);
            return Ok(response);
        }

        [HttpPost("active-tenant")]
        public async Task<IActionResult> ActiveTenant([FromBody] ActiveTenantParam param)
        {
            var response = await _tenantService.ActiveTenant(param.TenantId);
            return Ok(response);
        }
    }
}
