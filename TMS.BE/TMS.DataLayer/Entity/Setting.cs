using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;

namespace TMS.DataLayer.Entity
{
    [Table("settings", "")]
    public class Setting
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime ThesisRegistrationFromDate { get; set; }
        public DateTime ThesisRegistrationToDate { get; set; }
        public DateTime ThesisEditTitleFromDate { get; set; }
        public DateTime ThesisEditTitleToDate { get; set; }

    }
}
