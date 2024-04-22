using AutoMapper;
using TMS.BusinessLayer.DTO;
using TMS.DataLayer.Entity;

namespace TMS.BusinessLayer.Profiles
{
    public class FacultyProfile : Profile
    {
        public FacultyProfile()
        {
            CreateMap<Faculty, FacultyDto>();
            CreateMap<FacultyDto, Faculty>();
        }
    }
}
