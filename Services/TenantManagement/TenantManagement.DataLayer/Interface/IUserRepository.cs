using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantManagement.DataLayer.Entity;
using TMS.BaseRepository;
using TMS.DataLayer.Entity;

namespace TenantManagement.DataLayer.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsername(string username);
        Task<bool> Update(User user);
    }
}
