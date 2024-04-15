using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BusinessLayer.DTO
{
    public class UploadResult
    {
        public int RowsSuccess { get; set; }
        public List<RowErrorDetail> RowsError { get; set; }
    }
}
