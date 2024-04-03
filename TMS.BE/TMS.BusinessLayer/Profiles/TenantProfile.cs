using AutoMapper;
using TMS.BusinessLayer.DTO;
using TMS.DataLayer.Entity;

namespace TMS.BusinessLayer.Profiles
{
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            CreateMap<Tenant, TenantDto>();
            CreateMap<Tenant, TenantLiteDto>();
            CreateMap<TenantDto, Tenant>();

        }
    }
}
