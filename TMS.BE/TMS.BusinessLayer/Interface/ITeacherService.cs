using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;

namespace TMS.BusinessLayer.Interface
{
    public interface ITeacherService: IBaseService<TeacherDto>
    {
        Task<ServiceResponse<UploadResult>> HanleUploadFileAsync(IFormFile file);

        Task<byte[]> GetSampleUploadFile();

    }
}
