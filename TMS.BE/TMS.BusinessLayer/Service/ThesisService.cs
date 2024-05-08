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
using TMS.DataLayer.Param;

namespace TMS.BusinessLayer.Service
{
    public class ThesisService : BaseService<Thesis, ThesisDto>, IThesisService
    {
        private readonly IThesisRepository _thesisRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;

        public ThesisService(IThesisRepository thesisRepository, IStudentService studentService, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(thesisRepository, mapper, unitOfWork)
        {
            _thesisRepository = thesisRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _studentService = studentService;

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
            try
            {
                await _unitOfWork.OpenAsync();
                var newThesisCode = await _thesisRepository.GetNewThesisCode();
                var studentId = new Guid(_httpContextAccessor.HttpContext.User.FindFirst("UserId").Value);
                var studentName = _httpContextAccessor.HttpContext.User.FindFirst("FullName").Value;
                var student = await _studentService.GetByIdAsync(studentId.ToString());
                return new ThesisDto()
                {
                    ThesisId = Guid.NewGuid(),
                    ThesisCode = newThesisCode,
                    StudentId = studentId,
                    StudentName = studentName,
                    TeacherId = null,
                    Year = DateTime.Now.Year,
                    Semester = DateTime.Now.Month >= 6 ? 1 : 2,
                    FacultyCode = student.FacultyCode,
                    Status = ThesisStatus.WaitingForApproval
                };
            } catch
            {
                throw;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }

        }

        public override async Task AfterCreate(ThesisDto thesisDto)
        {
            if (thesisDto.CoTeachers != null && thesisDto.CoTeachers.Count > 0)
            {
                await SaveCoTeacher(thesisDto);
            }

        }

        public override async Task AfterUpdate(ThesisDto thesisDto)
        {
            if (thesisDto.CoTeachers != null && thesisDto.CoTeachers.Count > 0)
            {
                await SaveCoTeacher(thesisDto);
            }
        }

        public override async Task AfterDelete(ThesisDto thesisDto)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                await _thesisRepository.RemoveAllThesisCoTeacher(thesisDto.ThesisId);
                await _unitOfWork.CommitAsync();
            } catch
            {
                throw;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }
        }  
        
 

        public async Task SaveCoTeacher(ThesisDto thesisDto)
        {
            // save thesis co teachers
            try
            {
                await _unitOfWork.OpenAsync();

                foreach (var coTeacher in thesisDto.CoTeachers)
                {
                    if (coTeacher.State == EntityState.Create)
                    {
                        await _thesisRepository.AddThesisCoTeacher(thesisDto.ThesisId, coTeacher.TeacherId);
                    }

                    if (coTeacher.State == EntityState.Delete)
                    {
                        await _thesisRepository.RemoveThesisCoTeacher(thesisDto.ThesisId, coTeacher.TeacherId);
                    }

                }

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
        }

        public async Task<ServiceResponse<List<ThesisDto>>> GetGuidingList(string teacherId)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                var listCoGuiding = await _thesisRepository.GetListCoGuiding(teacherId);
                var listGuiding = await _thesisRepository.GetListGuiding(teacherId);
                
                var list = listCoGuiding.Concat(listGuiding).ToList();
                var mappedList = _mapper.Map<List<ThesisDto>>(list);
                var result =  new ServiceResponse<List<ThesisDto>>()
                {
                    Data = mappedList,
                    Message = "Success",
                    Success = true
                };
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }

        }

        public async Task<(List<ThesisDto>, int)> GetListThesisGuided(TeacherThesisFilterParam param)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                var result = await _thesisRepository.GetListThesisGuided(param);
                var mappedList = _mapper.Map<List<ThesisDto>>(result.Item1);
                return (mappedList, result.Item2);
            } catch { throw; }
            finally
            {
                await _unitOfWork.CloseAsync();
            }
        }
    }
}
