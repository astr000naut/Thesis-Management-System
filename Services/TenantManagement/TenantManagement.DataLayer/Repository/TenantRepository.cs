using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantManagement.DataLayer.Interface;
using TMS.BaseRepository;
using TMS.DataLayer.Entity;

namespace TenantManagement.DataLayer.Repository
{
    public class TenantRepository : BaseRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
