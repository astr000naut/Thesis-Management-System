using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.DataLayer.Entity;
using TMS.DataLayer.Interface;

namespace TMS.DataLayer.Repository
{
    public class TenantRepository : ITenantRepository
    {
        private readonly TenantManagementContext _tmcontext;
        public TenantRepository(TenantManagementContext tenantManagementContext) 
        {
            _tmcontext = tenantManagementContext;
        }

        public async Task<Tenant?> GetTenantByDomain(string domain)
        {
            // create connection from _connectionString use mysql connection
            // create query to get tenant by domain
            // execute query and return result
            using (var connection = _tmcontext.CreateConnection())
            {

                string query = $"SELECT * FROM tenants WHERE Domain = '{domain}'";
                var result = await connection.QueryAsync<Tenant>(query);
                return result.FirstOrDefault();
            }

        }

        public async Task<string> GetTenantConnectionString(string tenantId)
        {
            using (var connection = _tmcontext.CreateConnection())
            {
                string query = $"SELECT DBConnection, DBName FROM tenants WHERE TenantId = '{tenantId}'";
                var result = await connection.QueryAsync<(string, string)>(query);
                if (result == null || result.Count() == 0)
                {
                    throw new Exception("Tenant connection string not found");
                }
                var connectionString = result.FirstOrDefault().Item1 + "Database=" + result.FirstOrDefault().Item2+";";
                return connectionString;

            }
        }   
    }
}
