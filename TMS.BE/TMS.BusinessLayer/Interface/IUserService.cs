using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;

namespace TMS.BusinessLayer.Interface
{
    public interface IUserService
    {
        Task<ServiceResponse<LoginResponseDto>> Login(string username, string password);

        Task<ServiceResponse<bool>> RefreshToken(string accessToken, string refreshToken);
        Task<ServiceResponse<bool>> ChangePasswordAsync(string oldPass, string newPass, string confirmPass);
    }
}
