using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;
using TMS.DataLayer.Param;

namespace TMS.BusinessLayer.Interface
{
    public interface IThesisService: IBaseService<ThesisDto>
    {
        Task<ServiceResponse<List<ThesisDto>>> GetGuidingList(string teacherId);

        Task<(List<ThesisDto>, int)> GetListThesisGuided(TeacherThesisFilterParam param);
    }
}
