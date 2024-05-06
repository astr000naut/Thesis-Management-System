using Microsoft.AspNetCore.Mvc;
using TenantManagement.API.Param;
using TenantManagement.BusinessLayer.DTO;
using TenantManagement.BusinessLayer.Interface;
using TMS.BaseService;

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

        [HttpPost("check-connection")]
        public async Task<ServiceResponse<bool>> CheckDBConnection([FromBody] CheckConnectionParam param)
        {
            var response = await _tenantService.CheckConnection(param);
            return response;
        }

        [HttpPost("remove-resource")]
        public async Task<ServiceResponse<bool>> RemoveResource([FromBody] TenantDto tenantDto)
        {
            var response = await _tenantService.RemoveTenantResourceAsync(tenantDto);
            return response;
        }

        [HttpPost("active-tenant")]
        public async Task<IActionResult> ActiveTenant([FromBody] ActiveTenantParam param)
        {
            var response = await _tenantService.ActiveTenant(param.TenantId);
            return Ok(response);
        }
    }
}
