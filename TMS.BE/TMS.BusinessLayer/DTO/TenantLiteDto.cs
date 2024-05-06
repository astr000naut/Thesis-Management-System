using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;

namespace TMS.BusinessLayer.DTO
{
    public class TenantLiteDto
    {
        [Key]
        public Guid? TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantCode { get; set; }

        public string Domain { get; set; }

        public int Status { get; set; }
       
        public string? LogoUrl { get; set; }
  
    }
}
