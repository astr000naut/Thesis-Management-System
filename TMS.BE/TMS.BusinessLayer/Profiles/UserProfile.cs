using AutoMapper;
using TMS.BusinessLayer.DTO;
using TMS.DataLayer.Entity;


namespace TMS.BusinessLayer.Profiles
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
