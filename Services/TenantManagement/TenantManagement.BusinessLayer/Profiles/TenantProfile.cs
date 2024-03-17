using AutoMapper;
using TenantManagement.BusinessLayer.DTO;
using TMS.DataLayer.Entity;

namespace TenantManagement.BusinessLayer.Profiles
{
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            CreateMap<Tenant, TenantDto>();
            CreateMap<TenantDto, Tenant>();

        }
    }
}
