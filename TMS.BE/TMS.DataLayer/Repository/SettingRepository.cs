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
    }
}
