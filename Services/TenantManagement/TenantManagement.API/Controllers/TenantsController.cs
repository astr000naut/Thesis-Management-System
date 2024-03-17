using Microsoft.AspNetCore.Mvc;
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

    }
}
