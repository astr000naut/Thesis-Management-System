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
    }
}
