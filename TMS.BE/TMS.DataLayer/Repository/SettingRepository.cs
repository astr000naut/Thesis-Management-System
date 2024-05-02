using Dapper;
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
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {

        public SettingRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {}

        public async Task<Setting?> GetSettingAsync()
        {
            // select setting from settings table order by id desc limit 1
            var query = "SELECT * FROM settings ORDER BY Id DESC LIMIT 1;";
            var result = await _unitOfWork.Connection.QueryAsync<Setting>(sql: query, transaction: _unitOfWork.Transaction);            
            return result.FirstOrDefault();
        }
    }
}
