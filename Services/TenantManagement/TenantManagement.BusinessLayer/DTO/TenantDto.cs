using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;

namespace TenantManagement.BusinessLayer.DTO
{
    public class TenantDto
    {
        [Key]
        public Guid TenantId { get; set; }

        [Required]
        [StringLength(255)]
        public string TenantName { get; set; }

        [Required]
        [StringLength(255)]
        public string TenantCode { get; set; }

        [StringLength(255)]
        public string LogoUrl { get; set; }

        [StringLength(255)]
        public string ConnectionString { get; set; }
    }
}
