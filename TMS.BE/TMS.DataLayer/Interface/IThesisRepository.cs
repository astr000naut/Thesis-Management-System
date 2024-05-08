using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.DataLayer.Entity;
using TMS.DataLayer.Param;

namespace TMS.DataLayer.Interface
{
    public interface IThesisRepository : IBaseRepository<Thesis>
    {
        Task<string> GetNewThesisCode();

        Task<bool> AddThesisCoTeacher(Guid thesisId, Guid teacherId);

        Task<bool> RemoveThesisCoTeacher(Guid thesisId, Guid teacherId);

        Task<bool> RemoveAllThesisCoTeacher(Guid thesisId);

        Task<List<Thesis>> GetListGuiding(string teacherId);
        Task<List<Thesis>> GetListCoGuiding(string teacherId);

        Task<(List<Thesis>, int)> GetListThesisGuided(TeacherThesisFilterParam param);
    }
}
