using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.BaseRepository.Param;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.BusinessLayer.Param;
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

        public override async Task<byte[]> ExportExcelAsync(ExportParam exportParam)
        {
            try
            {
                await _unitOfWork.OpenAsync();

                var filterParam = new FilterParam()
                {
                    Skip = 0,
                    Take = 0,
                    CustomWhere = exportParam.CustomWhere,
                    KeySearch = exportParam.KeySearch,
                    FilterColumns = exportParam.FilterColumns
                };

                var filterResult = await FilterAsync(filterParam);
                var totalEntity = filterResult.Item2;
                var entityDtoListRaw = filterResult.Item1;

                var entityDtoList = new List<ThesisDto>();

                foreach (var eDto in entityDtoListRaw)
                {
                    var entityDtoFull = await GetByIdAsync(eDto.ThesisId.ToString());
                    entityDtoList.Add(entityDtoFull);
                }

                


                ExcelPackage excel = new ExcelPackage();

                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                ExcelWorksheet ws = excel.Workbook.Worksheets[0];

                ws.Cells.Style.Font.Size = 13;
                ws.Cells.Style.Font.Name = "Times New Roman";
                ws.Rows.CustomHeight = false;
                ws.Cells.Style.Indent = 1;
                // Bật wrap text cho tất cả các cell
                ws.Cells.Style.WrapText = true;

                //Số lượng cột của header
                var countColHeader = exportParam.Columns.Count;

                // merge các column lại từ col 1 đến số col header để tạo heading
                ws.Cells[1, 1].Value = exportParam.TableHeading;
                ws.Cells[1, 1, 1, countColHeader].Merge = true;

                // in đậm heading
                ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;


                int colIndex = 1;
                int rowIndex = 3;

                // tạo các header 
                for (int i = 0; i < exportParam.Columns.Count; ++i)
                {
                    var item = exportParam.Columns[i];
                    var cell = ws.Cells[rowIndex, colIndex];

                    //set màu thành gray
                    var fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    //căn chỉnh các border
                    var border = cell.Style.Border;
                    border.Bottom.Style =
                        border.Top.Style =
                        border.Left.Style =
                        border.Right.Style = ExcelBorderStyle.Thin;

                    //Độ rộng của các cột
                    ws.Columns[i + 1].Width = item.Width;

                    // In đậm
                    ws.Cells[2, i + 1].Style.Font.Bold = true;

                    // align text
                    ws.Columns[i + 1].Style.HorizontalAlignment = item.Align switch
                    {
                        "left" => ExcelHorizontalAlignment.Left,
                        "right" => ExcelHorizontalAlignment.Right,
                        _ => ExcelHorizontalAlignment.Center,
                    };

                    cell.Value = item.Caption;
                    ++colIndex;
                }

                // căn giữa heading
                ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // đổ dữ liệu vào sheet
                foreach (var entityDto in entityDtoList)
                {
                    ++rowIndex;
                    colIndex = 1;
                    ws.Cells[rowIndex, colIndex++].Value = rowIndex - 2;

                    for (int i = 1; i < exportParam.Columns.Count; i++)
                    {
                        object colValue;

                        // custom data
                        if (exportParam.Columns[i].Name == "CoTeachers")
                        {
                            colValue = string.Join(" - ", entityDto.CoTeachers.Select(x => x.TeacherName).ToArray());
                        } else
                        {
                            colValue = entityDto?.GetType().GetProperty(exportParam.Columns[i].Name)?.GetValue(entityDto);
                        }


                        switch (exportParam.Columns[i].Type)
                        {
                            case "number":
                                long value = (long)colValue;
                                ws.Cells[rowIndex, colIndex++].Value = value.ToString("N0", new CultureInfo("vi-VN")); ;
                                break;
                            case "date":
                                DateTime date = (DateTime)colValue;
                                ws.Cells[rowIndex, colIndex++].Value = date.ToString("dd/MM/yyyy");
                                break;
                            default:
                                ws.Cells[rowIndex, colIndex++].Value = colValue;
                                break;

                        }

                    }
                }

                // Thêm border cho các cells
                if (totalEntity > 0)
                {
                    var cellBorder = ws.Cells[3, 1, 3 + totalEntity, exportParam.Columns.Count].Style.Border;
                    cellBorder.Bottom.Style =
                        cellBorder.Top.Style =
                        cellBorder.Left.Style =
                        cellBorder.Right.Style = ExcelBorderStyle.Thin;
                }

                byte[] bin = excel.GetAsByteArray();
                await _unitOfWork.CommitAsync();
                return bin;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }

        }
    }
}
