using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantManagement.API.Param;
using TenantManagement.BusinessLayer.DTO;
using TMS.BaseService;

namespace TenantManagement.BusinessLayer.Interface
{
    public interface ITenantService: IBaseService<TenantDto>
    {
        Task<ServiceResponse<bool>> CheckConnection(CheckConnectionParam param);
        Task<TenantDto> ActiveTenant(Guid tenantId);

        Task<ServiceResponse<bool>> RemoveTenantResourceAsync(TenantDto tenantDto);
    }
}
