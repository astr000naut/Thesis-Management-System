using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;

namespace TMS.DataLayer.Entity
{
    [Table("tenants", "")]
    public class Tenant
    {
        [Key]
        public Guid? TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantCode { get; set; }

        public string? DBConnection { get; set; }
        public string DBName { get; set; }

        public string SurrogateName { get; set; }
        public string SurrogatePhoneNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string SurrogateEmail { get; set; }
        public string Domain { get; set; }
        public bool AutoCreateDB { get; set; }
        public int Status { get; set; }
        public string LogoUrl { get; set; }
        public bool AutoCreateMinio { get; set; }
        public string MinioEndpoint { get; set; }
        public int MinioPort { get; set; }
        public string MinioAccessKey { get; set; }
        public string MinioSecretKey { get; set; }
        public string MinioBucketName { get; set; }

    }

}
