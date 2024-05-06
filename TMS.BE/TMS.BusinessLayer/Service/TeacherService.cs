using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Minio.DataModel.Args;
using Minio;
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
using TMS.DataLayer.Interface;
using TMS.BusinessLayer.Infra;

namespace TMS.BusinessLayer.Service
{
    public class TeacherService : BaseService<Teacher, TeacherDto>, ITeacherService
    {
        private readonly ITeacherRepository _TeacherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public TeacherService(ITeacherRepository TeacherRepository, IUserRepository userRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(TeacherRepository, mapper, unitOfWork)
        {
            _TeacherRepository = TeacherRepository;
            _userRepository = userRepository;
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



        public async Task<ValidateUploadResult<Teacher>> ValidateFileUploadAsync(IFormFile file)
        {
            var result = new ValidateUploadResult<Teacher>()
            {
                ValidData = new List<Teacher>(),
                RowsError = new List<RowErrorDetail>(),
            };

            List<(int, Teacher)> rowValid = new List<(int, Teacher)> ();

            try
            {
                await _unitOfWork.OpenAsync();

                // Read the Excel file content
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        // Access the Excel workbook, worksheets, cells, etc. here
                        // Example:
                        var worksheet = package.Workbook.Worksheets[0];

                        int rowStart = 4,
                            rowEnd = worksheet.Dimension.End.Row,
                            colIndex = 1,
                            colStart = 2,
                            colEnd = 7;

                        List<(string, string)> dataFormat = new List<(string, string)>() {
                            ("TeacherCode", "require"),
                            ("TeacherName", "require"),
                            ("FacultyCode", "require"),
                            ("FacultyName", ""),
                            ("Email", ""),
                            ("PhoneNumber", "")
                        };

                        bool filledData;
                        for (int row = rowStart; row <= rowEnd; row++)
                        {
                            // check tồn tại số thứ tự
                            if (worksheet.Cells[row, colIndex].Value == null)
                            {
                                break;
                            }

                            var teacher = new Teacher();
                            teacher.UserId = Guid.NewGuid();

                            filledData = true;

                            for (int col = colStart; col <= colEnd; col++)
                            {
                                var cellValue = worksheet.Cells[row, col].Value.ToString();
                                var property = dataFormat[col - colStart];

                                if (property.Item2 == "require" && string.IsNullOrEmpty(cellValue))
                                {
                                    result.RowsError.Add(new RowErrorDetail()
                                    {
                                        RowIndex = row,
                                        ErrorMessage = $"Thiếu dữ liệu tại dòng [{row}], cột [{col}]"
                                    });
                                    filledData = false;
                                    break;
                                }
                                teacher.GetType().GetProperty(property.Item1).SetValue(teacher, cellValue);
                            }

                            // Nếu xuất hiện lỗi thì bỏ qua dòng này
                            if (!filledData)
                            {
                                continue;
                            }

                            rowValid.Add((row, teacher));
                        }

                        // Kiểm tra xem giảng viên đã tồn tại chưa
                        var listTeacherCode = rowValid.Select(x => x.Item2.TeacherCode).ToList();
                        var existedTeacher = await _TeacherRepository.GetTeacherByListTeacherCode(listTeacherCode);
                        if (existedTeacher.Count > 0)
                        {

                            foreach (var teacher in existedTeacher)
                            {
                                var row = rowValid.Find(x => x.Item2.TeacherCode == teacher.TeacherCode).Item1;
                                result.RowsError.Add(new RowErrorDetail()
                                {
                                    RowIndex = row,
                                    ErrorMessage = $"Giảng viên [{teacher.TeacherCode}] đã tồn tại"
                                });

                                // Xóa khỏi rowValid
                                rowValid.RemoveAll(x => x.Item2.TeacherCode == teacher.TeacherCode);
                            }

                            result.RowsError = result.RowsError.OrderBy(x => x.RowIndex).ToList();
                        }
                        result.ValidData = rowValid.Select(x => x.Item2).ToList();
                    }
                }
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }

        }

        public async Task<byte[]> GetSampleUploadFile()
        {
            try
            {
                var res = Array.Empty<byte>();

                IMinioService minioService = new MinioService(_httpContextAccessor);
                res = await minioService.DownloadObjectAsync("teacher_upload.xlsx");

                return res;
            }
            catch (Exception e)
            {
                throw new Exception("Error while getting sample upload file");
            }

        }

        public async Task<ServiceResponse<UploadResult>> HanleUploadFileAsync(IFormFile file)
        {
            var response = new ServiceResponse<UploadResult>();
            var uploadResult = new UploadResult()
            {
                RowsSuccess = 0,
                RowsError = new List<RowErrorDetail>()
            };

            
            try
            {
                await _unitOfWork.OpenAsync();

                // Validate the uploaded Excel file
                var validateResult = await ValidateFileUploadAsync(file);

                // If there are errors, return the error message
                if (validateResult.RowsError.Count > 0)
                {

                    uploadResult.RowsError = validateResult.RowsError;
                    response.Success = false;
                    response.Message = "Error processing Excel file";
                    response.Data = uploadResult;
                    return response;
                }   
                // If there are no errors, save the data to the database
                if (validateResult != null)
                {                   
                    foreach (var teacher in validateResult.ValidData)
                    {
                        var user = new User()
                        {
                            UserId = teacher.UserId,
                            FullName = teacher.TeacherName,
                            Username = teacher.TeacherCode,
                            Password = BCrypt.Net.BCrypt.HashPassword(teacher.TeacherCode),
                            Role = "Teacher"
                        };
                        await _userRepository.CreateAsync(user);
                        await _TeacherRepository.CreateAsync(teacher);
                        uploadResult.RowsSuccess++;
                    }          
                }
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Excel file processed successfully";
                response.Data = uploadResult;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error processing Excel file: " + ex.Message;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }

            return response;
        }

        public override async Task<int> DeleteMultipleAsync(List<string> idList)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                int teacherDeleted = await _TeacherRepository.DeleteMultipleAsync(idList);
                int userDeleted = await _userRepository.DeleteMultipleAsync(idList);
                if (teacherDeleted == userDeleted)
                {
                    await _unitOfWork.CommitAsync();
                    return teacherDeleted;
                } else
                {
                    throw new Exception("Error while deleting teacher");
                }
            } catch
            {
                throw;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }
        }
    }
}
