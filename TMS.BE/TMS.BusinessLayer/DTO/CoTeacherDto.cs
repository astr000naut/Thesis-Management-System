using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataLayer.Enum;

namespace TMS.BusinessLayer.DTO
{
    public class CoTeacherDto
    {
        [Key]
        public Guid? Id { get; set; }
        public Guid ThesisId { get; set; }
        public Guid TeacherId { get; set; }
        [NotMapped]
        public string? TeacherCode { get; set; }
        [NotMapped]
        public string? TeacherName { get; set; }

        [NotMapped]
        public EntityState State { get; set; }
    }
}
