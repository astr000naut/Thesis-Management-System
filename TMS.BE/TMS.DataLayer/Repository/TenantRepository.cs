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

                string query = $"SELECT * FROM tenants WHERE Domain = @domain";
                var result = await connection.QueryAsync<Tenant>(query, new { domain });
                return result.FirstOrDefault();
            }

        }

        public async Task<Tenant?> GetTenantById(string id)
        {
            using (var connection = _tmcontext.CreateConnection())
            {

                string query = $"SELECT * FROM tenants WHERE TenantId=@id";
                var result = await connection.QueryAsync<Tenant>(query, new { id });
                return result.FirstOrDefault();
            }

        } 
    }
}
