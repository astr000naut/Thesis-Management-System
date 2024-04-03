using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BusinessLayer.DTO;
using TMS.BaseService;

namespace TMS.BusinessLayer.Interface
{
    public interface ITenantService
    {
        Task<TenantLiteDto> GetTenantBaseInfo(string domain);
        Task<string> GetTenantConnectionString(string tenantId);
    }
}
