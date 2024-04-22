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
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {

        public TeacherRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {}

        public async Task<List<Teacher>> GetTeacherByListTeacherCode(List<string> listCode)
        {
            var query = "SELECT * FROM teachers WHERE TeacherCode IN @ListCode";
            var parameters = new { ListCode = listCode };
            var result = await _unitOfWork.Connection.QueryAsync<Teacher>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result.ToList();
        }
    }
}
