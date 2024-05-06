using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.BaseService;
using TMS.DataLayer.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Dapper;
using Newtonsoft.Json;
using TMS.DataLayer.Interface;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using System.Security.Claims;
using TMS.DataLayer.Repository;

namespace TMS.BusinessLayer.Service
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;

        public TenantService(ITenantRepository tenantRepository, IMapper mapper)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
        }

        public async Task<TenantLiteDto> GetTenantBaseInfo(string domain)
        {
            try
            {
                var tenant = await _tenantRepository.GetTenantByDomain(domain);
                var tenantLite = _mapper.Map<TenantLiteDto>(tenant);
                return tenantLite;
            } catch
            {
                throw new Exception("Tenant not found");
            }
        }

        public async Task<TenantDto> GetTenantByIdAsync(string tenantId)
        {
            try
            {
                var tenant = await _tenantRepository.GetTenantById(tenantId);
                var tenantDto = _mapper.Map<TenantDto>(tenant);
                return tenantDto;
            }
            catch
            {
                throw new Exception("Error while get conection string");
            }
        }



    }
}


