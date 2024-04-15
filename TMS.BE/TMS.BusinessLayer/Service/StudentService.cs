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
using TMS.DataLayer.Interface;

namespace TMS.BusinessLayer.Service
{
    public class StudentService : BaseService<Student, StudentDto>, IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IStudentRepository studentRepository, IUserRepository userRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(studentRepository, mapper, unitOfWork)
        {
            _studentRepository = studentRepository;
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



        public async Task<ValidateUploadResult<Student>> ValidateFileUploadAsync(IFormFile file)
        {
            var result = new ValidateUploadResult<Student>()
            {
                ValidData = new List<Student>(),
                RowsError = new List<RowErrorDetail>(),
            };

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
                            colEnd = 10;

                        List<(string, string)> dataFormat = new List<(string, string)>() {
                            ("StudentCode", "require"),
                            ("StudentName", "require"),
                            ("FacultyCode", "require"),
                            ("FacultyName", "require"),
                            ("Major", "require"),
                            ("Class", "require"),
                            ("GPA", ""),
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

                            var student = new Student();
                            student.UserId = Guid.NewGuid();

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
                                student.GetType().GetProperty(property.Item1).SetValue(student, cellValue);
                            }

                            // Nếu xuất hiện lỗi thì bỏ qua dòng này
                            if (!filledData)
                            {
                                continue;
                            }

                            // Kiểm tra xem sinh viên đã tồn tại chưa
                            var existedStudent = await _studentRepository.FilterAsync(0, 1, student.StudentCode, new List<string>() { "StudentCode" });
                            if (existedStudent.total > 0)
                            {
                                result.RowsError.Add(new RowErrorDetail()
                                {
                                    RowIndex = row,
                                    ErrorMessage = $"Dữ liệu đã tồn tại"
                                });
                                continue;
                            }

                            result.ValidData.Add(student);
                        }
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
                    foreach (var student in validateResult.ValidData)
                    {
                        var user = new User()
                        {
                            UserId = student.UserId,
                            FullName = student.StudentName,
                            Username = student.StudentCode,
                            Password = BCrypt.Net.BCrypt.HashPassword(student.StudentCode),
                            Role = "Student"
                        };
                        await _userRepository.CreateAsync(user);
                        await _studentRepository.CreateAsync(student);
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

        public async Task<ServiceResponse<UploadResult>> ValidateUploadAsync(IFormFile file)
        {
            var validateResult = await ValidateFileUploadAsync(file);
            var response = new ServiceResponse<UploadResult>();
            response.Data.RowsSuccess = validateResult.ValidData.Count;
            response.Data.RowsError = validateResult.RowsError;
            return response;
        }
    }
}
