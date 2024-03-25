using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;

namespace TMS.DataLayer.Entity
{
    [Table("tenants")]
    public class Tenant
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantCode { get; set; }
        public string DBConnection { get; set; }
        public string SurrogateName { get; set; }
        public string SurrogatePhoneNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string SurrogateEmail { get; set; }
        public bool AutoCreateDB { get; set; }
        public string Domain { get; set; }
        public int Status { get; set; }

    }

}
