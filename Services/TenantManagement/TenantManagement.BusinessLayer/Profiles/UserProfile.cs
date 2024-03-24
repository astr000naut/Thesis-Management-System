using AutoMapper;
using TenantManagement.BusinessLayer.DTO;
using TenantManagement.DataLayer.Entity;
using TMS.DataLayer.Entity;

namespace TenantManagement.BusinessLayer.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

        }
    }
}
