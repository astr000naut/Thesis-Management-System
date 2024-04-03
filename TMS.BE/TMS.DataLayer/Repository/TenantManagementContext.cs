using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataLayer.Repository
{
    public class TenantManagementContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public TenantManagementContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("TenantManagement");
        }
        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
}
