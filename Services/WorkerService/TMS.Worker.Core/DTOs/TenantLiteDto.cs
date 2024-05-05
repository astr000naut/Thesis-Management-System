
using System.ComponentModel.DataAnnotations;


namespace TMS.Worker.Core
{
    public class TenantLiteDto
    {
        [Key]
        public Guid? TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantCode { get; set; }

        public string Domain { get; set; }
       
        public string? LogoUrl { get; set; }
  
    }
}
