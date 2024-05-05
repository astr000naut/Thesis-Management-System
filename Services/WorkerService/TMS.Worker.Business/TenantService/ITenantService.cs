using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Worker.Core;

namespace TMS.Worker.Business.TenantService
{
    public interface ITenantService
    {
        Task<TenantLiteDto> GetTenantBaseInfo(string domain);
        Task<string> GetTenantConnectionString(string tenantId);
    }
}
