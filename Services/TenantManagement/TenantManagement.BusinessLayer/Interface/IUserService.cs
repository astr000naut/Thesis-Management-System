using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantManagement.BusinessLayer.DTO;

namespace TenantManagement.BusinessLayer.Interface
{
    public interface IUserService
    {
        Task<LoginResponseDto> Login(string username, string password);

        Task<LoginResponseDto> RefreshToken(string accessToken, string refreshToken);
    }
}
