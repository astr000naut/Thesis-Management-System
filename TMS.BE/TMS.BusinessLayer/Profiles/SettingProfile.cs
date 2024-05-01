using AutoMapper;
using TMS.BusinessLayer.DTO;
using TMS.DataLayer.Entity;

namespace TMS.BusinessLayer.Profiles
{
    public class SettingProfile : Profile
    {
        public SettingProfile()
        {
            CreateMap<Setting, SettingDto>();
            CreateMap<SettingDto, Setting>();
        }
    }
}
