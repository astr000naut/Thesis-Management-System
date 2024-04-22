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
                      FacultyName varchar(100) NOT NULL DEFAULT '',
                      GPA varchar(100) DEFAULT '',
                      Email varchar(100) DEFAULT '',
                      PhoneNumber varchar(100) DEFAULT '',
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
                      FacultyName varchar(100) NOT NULL DEFAULT '',
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
                      StudentName varchar(100) NOT NULL DEFAULT '',

                      TeacherId char(36) NOT NULL DEFAULT '',
                      TeacherName varchar(100) NOT NULL DEFAULT '',

                      FacultyId char(36) NOT NULL DEFAULT '',     
                      FacultyName varchar(100) NOT NULL DEFAULT '',

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
                var url = _configuration.GetSection("Serviceurl:FileService").Value;
                url += "/active-tenant";
                bool isSuccess = false;
                using (var httpClient = new HttpClient())
                {
                    var requestBody = new StringContent(JsonConvert.SerializeObject(tenantDto), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(url, requestBody);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        isSuccess = JsonConvert.DeserializeObject<bool>(content);
                    } else
                    {
                        throw new Exception("Cannot connect to FileService");
                    }
                }
                return isSuccess;
                

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
    }
}


