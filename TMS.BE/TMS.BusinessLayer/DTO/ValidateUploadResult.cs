using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BusinessLayer.DTO
{
    public class ValidateUploadResult<TEntity>
    {
        public List<TEntity> ValidData { get; set; }
        public List<RowErrorDetail> RowsError { get; set; }
    }

    public class RowErrorDetail
    {
        public int RowIndex { get; set; }
        public string ErrorMessage { get; set; }
    }
}
