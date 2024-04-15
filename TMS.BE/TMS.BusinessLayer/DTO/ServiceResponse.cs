using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BusinessLayer.DTO
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; }
        public string Message { get; set; }

        public ServiceResponse()
        {
            Success = false;
            Message = null;
            Data = default(T);
        }
    }
}
