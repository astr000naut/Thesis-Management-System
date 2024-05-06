using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantManagement.BusinessLayer.DTO;
using TenantManagement.BusinessLayer.Interface;
using TenantManagement.DataLayer.Interface;
using TMS.BaseRepository;
using TMS.BaseService;
using TMS.DataLayer.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Dapper;
using Newtonsoft.Json;
using System.Security.Claims;
using TenantManagement.DataLayer.Entity;
using Minio;
using System.Net;
using TenantManagement.API.Param;
using Minio.DataModel.Args;

namespace TenantManagement.BusinessLayer.Service
{
    public class TenantService : BaseService<Tenant, TenantDto>, ITenantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantRepository _tenantRepository;
        private readonly IConfiguration _configuration;


        public TenantService(IConfiguration configuration, ITenantRepository tenantRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(tenantRepository, mapper, unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;

            var connectionString = configuration.GetConnectionString("SqlConnection");

            if (connectionString == null || connectionString.Length == 0)
            {
                throw new Exception("ConnectionString is not cofigured");
            }
            unitOfWork.SetConnectionString(connectionString);
        }


        public override async Task<TenantDto> GetNew()
        {
            var tenantDto = new TenantDto();
            tenantDto.TenantId = Guid.NewGuid();
            tenantDto.AutoCreateDB = true;
            tenantDto.AutoCreateMinio = true;
            tenantDto.DBConnection = _configuration.GetSection("DefaultConnection:ConnectionString").Value;
            tenantDto.DBName = "tenant-" + tenantDto.TenantId;
            tenantDto.MinioEndpoint = _configuration.GetSection("DefaultConnection:MinioEndpoint").Value;
            tenantDto.MinioPort = int.Parse(_configuration.GetSection("DefaultConnection:MinioPort").Value);
            tenantDto.MinioAccessKey = _configuration.GetSection("DefaultConnection:MinioAccessKey").Value;
            tenantDto.MinioSecretKey = _configuration.GetSection("DefaultConnection:MinioSecretKey").Value;
            tenantDto.MinioBucketName = "tenant-" + tenantDto.TenantId;

            return tenantDto;
        }

        public async Task<ServiceResponse<bool>> CheckConnection(CheckConnectionParam param)
        {
            var result = new ServiceResponse<bool>() { Data = true};
            var checkDbResult = await CheckDBConnection(param.AutoCreateDB, param.ConnectionString, param.DBName);
            if (!checkDbResult)
            {
                result.Data = false;
                result.ErrorCode = "DB_CONNECTION_ERROR";
                result.Message = "Không thể kết nối cơ sở dữ liệu";
            }
            var checkMinioResult = await CheckMinioConnection(param.AutoCreateMinio, param.MinioEndpoint, param.MinioAccessKey, param.MinioSecretKey, param.MinioBucketName);
            if (!checkMinioResult)
            {
                result.Data = false;
                result.ErrorCode = "MINIO_CONNECTION_ERROR";
                result.Message = "Không thể kết nối Minio";
            }
            return result;

        }

        public async Task<bool> CheckMinioConnection(bool autoCreateMinio, string endpoint, string accessKey, string secretKey, string bucketName)
        {
            IMinioClient minio;
            try
            {
                bool result = true;
                minio = new MinioClient()
                                    .WithEndpoint(endpoint)
                                    .WithCredentials(accessKey, secretKey)
                                    .Build();
                if (autoCreateMinio)
                {
                    var listBucket = await minio.ListBucketsAsync();
                } else
                {
                    // check if bucket exists
                    BucketExistsArgs buketExistArgs = new BucketExistsArgs();
                    buketExistArgs.WithBucket(bucketName);
                    result = await minio.BucketExistsAsync(buketExistArgs);
                }
                minio.Dispose();
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CheckDBConnection(bool autoCreateDB, string connectionString, string databaseName)
        {
            try
            {
                var connection = connectionString;
                var query = "SHOW DATABASES";

                if (!autoCreateDB)
                {
                    connection += "Database=" + databaseName + ";";
                    query = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '" + databaseName + "'";
                }
                
                // use mysql to check the connection to the database
                using (var conn = new MySqlConnector.MySqlConnection(connection))
                {
                    await conn.OpenAsync();
                    var result = await conn.QueryAsync(query);
                    await conn.CloseAsync();
                }   

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<TenantDto> ActiveTenant(Guid tenantId)
        {

            try
            {
                var tenant = await _tenantRepository.GetByIdAsync(tenantId);
                var tenantDto = _mapper.Map<TenantDto>(tenant);
                bool activeDatabaseSuccess = await ActiveTenantDatabase(tenantDto);
                if (!activeDatabaseSuccess)
                {
                    throw new Exception("Cannot active tenant database");
                }


                bool activeMinioSuccess = await ActiveTenantMinio(tenantDto);

                if (!activeMinioSuccess)
                {
                    throw new Exception("Cannot active tenant minio");
                }


                await _unitOfWork.OpenAsync();

                tenantDto.Status = 2; // Đang hoạt động;
                await UpdateAsync(tenantDto);

                await _unitOfWork.CommitAsync();

                return tenantDto;

            } catch (Exception e)
            {
                throw;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }
        }   


        private async Task<bool> GenerateDatabaseTable(MySqlConnector.MySqlTransaction transaction, TenantDto tenantDto)
        {
            try
            {
                // add use database to query
                string useDatabaseQuery = "USE `" + tenantDto.DBName + "`;";
                await transaction.Connection.ExecuteAsync(useDatabaseQuery, transaction: transaction); 

                string createUserTableQuery = @"
                    CREATE TABLE users (
                      UserId char(36) NOT NULL DEFAULT '',
                      Username varchar(100) NOT NULL DEFAULT '',
                      Password varchar(100) NOT NULL DEFAULT '',
                      FullName varchar(100) NOT NULL DEFAULT '',
                      Role varchar(100) NOT NULL DEFAULT '',
                      RefreshToken varchar(255) DEFAULT NULL,
                      RefreshTokenExp datetime DEFAULT NULL,
                      PRIMARY KEY (UserId)
                    )
                    ENGINE = INNODB,
                    AVG_ROW_LENGTH = 16384,
                    CHARACTER SET utf8,
                    COLLATE utf8_general_ci;
                ";

                await transaction.Connection.ExecuteAsync(createUserTableQuery, transaction: transaction);

                // Create index
                string createUserIndexQuery = @"
                    ALTER TABLE users
                    ADD UNIQUE INDEX Username (Username);
                ";

                await transaction.Connection.ExecuteAsync(createUserIndexQuery, transaction: transaction);

                // create students table

                string createStudentTableQuery = @"
                    CREATE TABLE students (
                      UserId char(36) NOT NULL DEFAULT '',
                      StudentCode varchar(100) NOT NULL DEFAULT '',
                      StudentName varchar(100) NOT NULL DEFAULT '',
                      Class varchar(100) NOT NULL DEFAULT '',
                      Major varchar(100) NOT NULL DEFAULT '',
                      FacultyCode varchar(100) NOT NULL DEFAULT '',
                      GPA varchar(100) DEFAULT '',
                      Email varchar(100) DEFAULT '',
                      PhoneNumber varchar(100) DEFAULT '',
                      Description text DEFAULT '',
                      PRIMARY KEY (UserId)
                    )
                    ENGINE = INNODB,
                    AVG_ROW_LENGTH = 16384,
                    CHARACTER SET utf8,
                    COLLATE utf8_general_ci;";
                await transaction.Connection.ExecuteAsync(createStudentTableQuery, transaction: transaction);

                // create index for students table and foreign key to users table
                string createStudentIndexQuery = @"
                    ALTER TABLE students
                    ADD UNIQUE INDEX StudentCode (StudentCode);";

                await transaction.Connection.ExecuteAsync(createStudentIndexQuery, transaction: transaction);

                string createStudentUserForeignKeyQuery = @"
                    ALTER TABLE students
                    ADD CONSTRAINT FK_Student_User FOREIGN KEY (UserId) REFERENCES users(UserId);
                ";

                await transaction.Connection.ExecuteAsync(createStudentUserForeignKeyQuery, transaction: transaction);

                // create teacher table

                string createTeacherTableQuery = @"
                    CREATE TABLE teachers (
                      UserId char(36) NOT NULL DEFAULT '',
                      TeacherCode varchar(100) NOT NULL DEFAULT '',
                      TeacherName varchar(100) NOT NULL DEFAULT '',
                      FacultyCode varchar(100) NOT NULL DEFAULT '',
                      Email varchar(100) DEFAULT '',
                      PhoneNumber varchar(100) DEFAULT '',
                      Description text DEFAULT '',
                      BeInstructor tinyint(1) DEFAULT 0,
                      PRIMARY KEY (UserId)
                    )
                    ENGINE = INNODB,
                    AVG_ROW_LENGTH = 16384,
                    CHARACTER SET utf8,
                    COLLATE utf8_general_ci;";
                await transaction.Connection.ExecuteAsync(createTeacherTableQuery, transaction: transaction);

                // create index for teachers table and foreign key to users table
                string createTeacherIndexQuery = @"
                    ALTER TABLE teachers
                    ADD UNIQUE INDEX TeacherCode (TeacherCode);";

                await transaction.Connection.ExecuteAsync(createTeacherIndexQuery, transaction: transaction);

                string createTeacherUserForeignKeyQuery = @"
                    ALTER TABLE teachers
                    ADD CONSTRAINT FK_Teacher_User FOREIGN KEY (UserId) REFERENCES users(UserId);
                ";

                await transaction.Connection.ExecuteAsync(createTeacherUserForeignKeyQuery, transaction: transaction);

                // create faculties table

                string createFacultyTableQuery = @"
                    CREATE TABLE faculties (
                      FacultyId char(36) NOT NULL DEFAULT '',
                      FacultyName varchar(100) NOT NULL DEFAULT '',
                      FacultyCode varchar(100) NOT NULL DEFAULT '',
                      Description text DEFAULT '',
                      PRIMARY KEY (FacultyId)
                    )
                    ENGINE = INNODB,
                    AVG_ROW_LENGTH = 16384,
                    CHARACTER SET utf8,
                    COLLATE utf8_general_ci;";
                await transaction.Connection.ExecuteAsync(createFacultyTableQuery, transaction: transaction);

                // create index for faculties table
                string createFacultyIndexQuery = @"
                    ALTER TABLE faculties
                    ADD UNIQUE INDEX FacultyCode (FacultyCode);";

                await transaction.Connection.ExecuteAsync(createFacultyIndexQuery, transaction: transaction);

                // create theses table
                string createThesesTableQuery = @"
                    CREATE TABLE theses (
                      ThesisId char(36) NOT NULL DEFAULT '',

                      ThesisCode varchar(100) NOT NULL DEFAULT '',
                      ThesisName varchar(100) NOT NULL DEFAULT '',
                      Description text DEFAULT '',

                      StudentId char(36) NOT NULL DEFAULT '',

                      TeacherId char(36) NOT NULL DEFAULT '',

                      FacultyCode varchar(100) NOT NULL DEFAULT '',

                      Year int(11) NOT NULL DEFAULT 0,
                      Semester int(11) NOT NULL DEFAULT 0,

                      ThesisFileUrl varchar(255) DEFAULT '',
                      ThesisFileName varchar(255) DEFAULT '',

                      Status int(11) NOT NULL DEFAULT 0,
                      PRIMARY KEY (ThesisId)
                    )
                    ENGINE = INNODB,
                    AVG_ROW_LENGTH = 16384,
                    CHARACTER SET utf8,
                    COLLATE utf8_general_ci;";
                await transaction.Connection.ExecuteAsync(createThesesTableQuery, transaction: transaction);

                // create index for theses table 
                string createThesisIndexQuery = @"
                    ALTER TABLE theses
                    ADD UNIQUE INDEX ThesisCode (ThesisCode);";

                await transaction.Connection.ExecuteAsync(createThesisIndexQuery, transaction: transaction);

                string createForeignKeyQuery = @"
                    ALTER TABLE theses
                    ADD CONSTRAINT FK_Thesis_Student FOREIGN KEY (StudentId) REFERENCES students(UserId);
                    ALTER TABLE theses
                    ADD CONSTRAINT FK_Thesis_Teacher FOREIGN KEY (TeacherId) REFERENCES teachers(UserId);
                ";

                await transaction.Connection.ExecuteAsync(createForeignKeyQuery, transaction: transaction);

                // create settings table
                string createSettingsTableQuery = @"
                    CREATE TABLE settings (
                      Id char(36) NOT NULL DEFAULT '',
                      ThesisRegistrationFromDate datetime DEFAULT NULL,
                      ThesisRegistrationToDate datetime DEFAULT NULL,
                      ThesisEditTitleFromDate datetime DEFAULT NULL,
                      ThesisEditTitleToDate datetime DEFAULT NULL,
                      PRIMARY KEY (Id)
                    )
                    ENGINE = INNODB,
                    AVG_ROW_LENGTH = 16384,
                    CHARACTER SET utf8,
                    COLLATE utf8_general_ci;";
                await transaction.Connection.ExecuteAsync(createSettingsTableQuery, transaction: transaction);

                // insert setting datastring
                var insertSettingsQuery = @"
                    INSERT INTO settings (Id, ThesisRegistrationFromDate, ThesisRegistrationToDate, ThesisEditTitleFromDate, ThesisEditTitleToDate)
                    VALUES ('" + Guid.NewGuid().ToString() + "', '2021-01-01 00:00:00', '2021-01-01 00:00:00', '2021-01-01 00:00:00', '2021-01-01 00:00:00');";
               
                await transaction.Connection.ExecuteAsync(insertSettingsQuery, transaction: transaction);


                // create view view_students
                string createViewStudentsQuery = @"
                    CREATE VIEW view_students AS
                    SELECT 
                        students.UserId,
                        students.StudentCode,
                        students.StudentName,
                        students.Class,
                        students.Major,
                        students.FacultyCode,
                        students.GPA,
                        students.Email,
                        students.PhoneNumber,
                        students.Description,
                        faculties.FacultyName
                    FROM students
                    LEFT JOIN faculties ON students.FacultyCode = faculties.FacultyCode;";
                await transaction.Connection.ExecuteAsync(createViewStudentsQuery, transaction: transaction);
                
                // create view view_teachers
                string createViewTeachersQuery = @"
                    CREATE VIEW view_teachers AS
                    SELECT 
                        teachers.UserId,
                        teachers.TeacherCode,
                        teachers.TeacherName,
                        teachers.FacultyCode,
                        teachers.Email,
                        teachers.PhoneNumber,
                        teachers.Description,
                        faculties.FacultyName
                    FROM teachers
                    LEFT JOIN faculties ON teachers.FacultyCode = faculties.FacultyCode;";

                await transaction.Connection.ExecuteAsync(createViewTeachersQuery, transaction: transaction);

                // create view view_theses
                string createViewThesesQuery = @"
                    CREATE VIEW view_theses AS
                    SELECT 
                        theses.ThesisId,
                        theses.ThesisCode,
                        theses.ThesisName,
                        theses.Description,
                        theses.StudentId,
                        theses.TeacherId,
                        theses.FacultyCode,
                        theses.Year,
                        theses.Semester,
                        theses.ThesisFileUrl,
                        theses.ThesisFileName,
                        theses.Status,
                        students.StudentCode,
                        students.StudentName,
                        teachers.TeacherCode,
                        teachers.TeacherName,
                        faculties.FacultyName
                    FROM theses
                    LEFT JOIN students ON theses.StudentId = students.UserId
                    LEFT JOIN teachers ON theses.TeacherId = teachers.UserId
                    LEFT JOIN faculties ON theses.FacultyCode = faculties.FacultyCode;";
                await transaction.Connection.ExecuteAsync(createViewThesesQuery, transaction: transaction);

                return true;
            } catch (Exception)
            {
                throw;
            }
        }

        private async Task<bool> GenerateInitialData(MySqlConnector.MySqlTransaction transaction, TenantDto tenantDto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("admin");

            string insertAdminUserQuery = @"
                INSERT INTO users (UserId, Username, Password, FullName, Role)
                VALUES ('" + Guid.NewGuid().ToString() + "', 'admin','"+ passwordHash +"', 'ADMIN', 'ADMIN')";

            await transaction.Connection.ExecuteAsync(insertAdminUserQuery, transaction: transaction); ;

            return true;

        }

        private async Task<bool> ActiveTenantDatabase(TenantDto tenantDto)
        {
            try
            {
                if (tenantDto.AutoCreateDB)
                {
                    var connection = tenantDto.DBConnection;
                    // drop database if exists then create new database
                    var query = "DROP DATABASE IF EXISTS `" + tenantDto.DBName + "`;";
                    query += "CREATE DATABASE IF NOT EXISTS `" + tenantDto.DBName + "`;";

                    using (var conn = new MySqlConnector.MySqlConnection(connection))
                    {
                        await conn.OpenAsync();
                        var _transaction = await conn.BeginTransactionAsync();
                        await conn.ExecuteAsync(query, transaction: _transaction);
                        await GenerateDatabaseTable(_transaction, tenantDto);
                        await GenerateInitialData(_transaction, tenantDto);
                        await _transaction.CommitAsync();
                        await conn.CloseAsync();
                    }
                }
                else
                {
                    var connection = tenantDto.DBConnection + "Database=" + tenantDto.DBName + ";";
                    using (var conn = new MySqlConnector.MySqlConnection(connection))
                    {
                        await conn.OpenAsync();
                        var _transaction = await conn.BeginTransactionAsync();
                        await GenerateDatabaseTable(_transaction, tenantDto);
                        await GenerateInitialData(_transaction, tenantDto);
                        await _transaction.CommitAsync();
                        await conn.CloseAsync();
                    }
                }
                return true;
            } catch (Exception)
            {
                return false;
            }
            
        }   

        private async Task<bool> ActiveTenantMinio(TenantDto tenantDto)
        {
           try
            {
                IMinioClient minio = new MinioClient()
                                    .WithEndpoint(tenantDto.MinioEndpoint)
                                    .WithCredentials(tenantDto.MinioAccessKey, tenantDto.MinioSecretKey)
                                    .Build();

                // create bucket if not exists
                if (tenantDto.AutoCreateMinio)
                {
                    MakeBucketArgs makeBucketArgs = new MakeBucketArgs();
                    makeBucketArgs.WithBucket(tenantDto.MinioBucketName);
                    await minio.MakeBucketAsync(makeBucketArgs);
                }

                // upload student_upload.xlsx

                var filePath = "D:\\Development\\TMS\\Services\\TenantManagement\\SeedData";
                var fileName = "student_upload.xlsx";
                var bucketName = tenantDto.MinioBucketName;
                var objectName = "student_upload.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";



                PutObjectArgs putObjectArgs = new PutObjectArgs();
                putObjectArgs.WithBucket(bucketName);
                putObjectArgs.WithObject(objectName);
                putObjectArgs.WithFileName(filePath + "\\" + fileName);
                putObjectArgs.WithContentType(contentType);
                await minio.PutObjectAsync(putObjectArgs);

                // upload teacher_upload.xlsx to minio
                fileName = "teacher_upload.xlsx";
                objectName = "teacher_upload.xlsx";
                putObjectArgs.WithObject(objectName);
                putObjectArgs.WithFileName(filePath + "\\" + fileName);
                await minio.PutObjectAsync(putObjectArgs);

                minio.Dispose();

                return true;

            } catch (Exception)
            {
                throw;
            }
     
        }

        public override async Task BeforeUpdate(TenantDto t)
        {

            var tenant = await _tenantRepository.GetByIdAsync(t.TenantId);
            if (tenant.Status == 2)
            {
                if (
                    t.DBConnection != tenant.DBConnection || t.DBName != tenant.DBName
                    || t.AutoCreateDB != tenant.AutoCreateDB || t.AutoCreateMinio != tenant.AutoCreateMinio
                    || t.Domain != tenant.Domain
                    || t.MinioAccessKey != tenant.MinioAccessKey || t.MinioBucketName != tenant.MinioBucketName
                    || t.MinioSecretKey != tenant.MinioSecretKey || t.MinioEndpoint != tenant.MinioEndpoint
                    )
                {
                    throw new Exception("Không thể cập nhật các thông tin kết nối khi khách hàng đang hoạt động");
                }   
            }

        }

        public async Task<ServiceResponse<bool>> RemoveTenantResourceAsync(TenantDto tenantDto)
        {
            var res = new ServiceResponse<bool>();
            try
            {
                var connection = tenantDto.DBConnection;
                // drop database if exists then create new database
                var query = "DROP DATABASE IF EXISTS `" + tenantDto.DBName + "`;";
             
                using (var conn = new MySqlConnector.MySqlConnection(connection))
                {
                    await conn.OpenAsync();
                    var _transaction = await conn.BeginTransactionAsync();
                    await conn.ExecuteAsync(query, transaction: _transaction);
                    await _transaction.CommitAsync();
                    await conn.CloseAsync();
                }

                IMinioClient minio = new MinioClient()
                                    .WithEndpoint(tenantDto.MinioEndpoint)
                                    .WithCredentials(tenantDto.MinioAccessKey, tenantDto.MinioSecretKey)
                                    .Build();
                // remove bucket
                BucketExistsArgs buketExistArgs = new BucketExistsArgs();
                buketExistArgs.WithBucket(tenantDto.MinioBucketName);

                bool found = await minio.BucketExistsAsync(buketExistArgs);
                if (found)
                {
                    // Remove bucket my-bucketname. This operation will succeed only if the bucket is empty.
                    RemoveBucketArgs removeBucketArgs = new RemoveBucketArgs();
                    removeBucketArgs.WithBucket(tenantDto.MinioBucketName);
                    await minio.RemoveBucketAsync(removeBucketArgs);
                }

                await _unitOfWork.OpenAsync();
                var tenant = await _tenantRepository.GetByIdAsync(tenantDto.TenantId);
                if (tenant != null)
                {
                    tenant.Status = 0;
                    await _tenantRepository.UpdateAsync(tenant);
                }
                await _unitOfWork.CommitAsync();

            } catch (Exception e)
            {
                throw;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }

            return res;
        }
    }
}


