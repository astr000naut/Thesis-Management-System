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


        public async Task<Student> GetStudentByStudentCodeAsync(string studentCode)
        {
            throw new NotImplementedException();
        }
    }
}
