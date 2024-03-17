using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.DataLayer.Entity;

namespace TenantManagement.DataLayer.Interface
{
    public interface ITenantRepository : IBaseRepository<Tenant>
    {
    }
}
