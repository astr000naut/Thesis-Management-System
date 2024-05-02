using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.DataLayer.Entity;
using TMS.DataLayer.Enum;
using TMS.DataLayer.Interface;
using TMS.DataLayer.Repository;

namespace TMS.BusinessLayer.Service
{
    public class SettingService : BaseService<Setting, SettingDto>, ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public SettingService(ISettingRepository settingRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(settingRepository, mapper, unitOfWork)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;

            // get connection string from httpcontext items

            var connectionString = _httpContextAccessor.HttpContext.Items["ConnectionString"].ToString();

            if (connectionString == null)
            {
                throw new Exception("ConnectionString is not cofigured");
            }

            _unitOfWork.SetConnectionString(connectionString);
        }

        public async Task<ServiceResponse<SettingDto>> GetSettingAsync()
        {
            var response = new ServiceResponse<SettingDto>();
            try
            {         
                await _unitOfWork.OpenAsync();
                var setting = await _settingRepository.GetSettingAsync();
                response.Data = _mapper.Map<SettingDto>(setting);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }
            return response;
        }

        public override async Task BeforeUpdate(SettingDto setting)
        {

            setting.ThesisRegistrationFromDate = setting.ThesisRegistrationFromDate!.Value.AddHours(7);
            setting.ThesisRegistrationToDate = setting.ThesisRegistrationToDate!.Value.AddHours(7);
            setting.ThesisEditTitleFromDate = setting.ThesisEditTitleFromDate!.Value.AddHours(7);
            setting.ThesisEditTitleToDate = setting.ThesisEditTitleToDate!.Value.AddHours(7);



        }
    }

}
