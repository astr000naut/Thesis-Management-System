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

        public string LogoUrl { get; set; }

        public string ConnectionString { get; set; }

    }

}
