using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantManagement.BusinessLayer.DTO;
using TMS.BaseService;

namespace TenantManagement.BusinessLayer.Interface
{
    public interface ITenantService: IBaseService<TenantDto>
    {
        Task<bool> CheckDBConnection(bool autoCreateDB, string connectionString, string databaseName);

        Task<TenantDto> ActiveTenant(Guid tenantId); 
    }
}
