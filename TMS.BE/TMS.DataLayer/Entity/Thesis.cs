using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataLayer.Enum;

namespace TMS.DataLayer.Entity
{
    [Common.Attribute.Table("theses", "view_theses")]
    public class Thesis
    {
        [Key]
        public Guid ThesisId { get; set; }
        public string ThesisCode { get; set; }
        public string ThesisName { get; set; }
        public string Description { get; set; }
        public Guid StudentId { get; set; }
        [NotMapped]
        public string StudentName { get; set; }
        [NotMapped]
        public string StudentCode { get; set; }
        public Guid TeacherId { get; set; }
        [NotMapped]
        public string TeacherName { get; set; }
        [NotMapped]
        public string TeacherCode { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public string ThesisFileUrl { get; set; }
        public string ThesisFileName { get; set; }
        public ThesisStatus Status { get; set; }
    }
}
