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
    public class ThesisRepository : BaseRepository<Thesis>, IThesisRepository
    {

        public ThesisRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {}

        public async Task<string> GetNewThesisCode()
        {
            var year = DateTime.Now.Year;
            var random = new Random();
            var newCode = $"KLTN_{year}_{random.Next(1, 10000):D4}";
            var query = "SELECT COUNT(*) FROM theses WHERE ThesisCode = @NewCode";
            var parameters = new { NewCode = newCode };
            var count = await _unitOfWork.Connection.ExecuteScalarAsync<int>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            if (count > 0)
            {
                return await GetNewThesisCode();
            }
            return newCode;
  
        }
    }
}
