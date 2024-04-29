using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BaseService
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; }
        public string Message { get; set; } = null;
        public string ErrorCode { get; set; } = null;
        public string ErrorDetail { get; set; } = null;

    }
}
