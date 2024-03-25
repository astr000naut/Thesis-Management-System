using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenantManagement.BusinessLayer.DTO
{
    public class FilterResponseDto
    {
        public UserDto User { get; set; }

        public string AccessToken { get; set; } 

        public string RefreshToken { get; set; }
    }
}
