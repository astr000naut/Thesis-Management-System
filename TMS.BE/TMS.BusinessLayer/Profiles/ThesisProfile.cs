using AutoMapper;
using TMS.BusinessLayer.DTO;
using TMS.DataLayer.Entity;

namespace TMS.BusinessLayer.Profiles
{
    public class ThesisProfile : Profile
    {
        public ThesisProfile()
        {
            CreateMap<Thesis, ThesisDto>();
            CreateMap<ThesisDto, Thesis>();
        }
    }
}
