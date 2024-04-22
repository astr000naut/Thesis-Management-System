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
    public class FacultyRepository : BaseRepository<Faculty>, IFacultyRepository
    {

        public FacultyRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {}
    }
}
