using AutoMapper;
using TMS.BusinessLayer.DTO;
using TMS.DataLayer.Entity;

namespace TMS.BusinessLayer.Profiles
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<Teacher, TeacherDto>();
            CreateMap<TeacherDto, Teacher>();
            CreateMap<CoTeacher, CoTeacherDto>();
            CreateMap<CoTeacherDto, CoTeacher>();
        }
    }
}
