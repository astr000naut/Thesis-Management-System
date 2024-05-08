using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;
using TMS.DataLayer.Enum;


namespace TMS.DataLayer.Entity
{
    [Common.Attribute.Table("thesis_co_guides", "view_thesis_co_guides")]
    public class CoTeacher
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ThesisId { get; set; }
        public Guid TeacherId { get; set; }
        [NotMapped]
        public string TeacherCode { get; set; }
        [NotMapped]
        public string TeacherName { get; set; }

        [NotMapped]
        public EntityState State { get; set; }

    }
}
