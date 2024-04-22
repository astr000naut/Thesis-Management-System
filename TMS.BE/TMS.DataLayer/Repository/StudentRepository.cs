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
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {

        public StudentRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {}

        public async Task<List<Student>> GetStudentByListStudentCode(List<string> listCode)
        {
            var query = "SELECT * FROM students WHERE StudentCode IN @ListCode";
            var parameters = new { ListCode = listCode };
            var result = await _unitOfWork.Connection.QueryAsync<Student>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result.ToList();
        }
    }
}
