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

namespace TMS.BusinessLayer.Service
{
    public class ThesisService : BaseService<Thesis, ThesisDto>, IThesisService
    {
        private readonly IThesisRepository _thesisRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public ThesisService(IThesisRepository thesisRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(thesisRepository, mapper, unitOfWork)
        {
            _thesisRepository = thesisRepository;
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

        public override async Task<ThesisDto> GetNew()
        {
            var newThesisCode = await _thesisRepository.GetNewThesisCode();
            var studentId = new Guid(_httpContextAccessor.HttpContext.User.FindFirst("UserId").Value);
            var studentName = _httpContextAccessor.HttpContext.User.FindFirst("FullName").Value;
            return new ThesisDto()
            {
                ThesisId = Guid.NewGuid(),
                ThesisCode = newThesisCode,
                StudentId = studentId,
                StudentName = studentName,
                TeacherId = null,
                Year = DateTime.Now.Year,
                Semester = DateTime.Now.Month >= 6 ? 1 : 2,
                Status = ThesisStatus.WaitingForApproval
            };

        }

    }
}
