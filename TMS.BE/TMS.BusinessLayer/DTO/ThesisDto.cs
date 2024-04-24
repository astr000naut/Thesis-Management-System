using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;
using TMS.DataLayer.Enum;

namespace TMS.BusinessLayer.DTO
{
    public class ThesisDto
    {
        [Key]
        public Guid ThesisId { get; set; }
        public string ThesisCode { get; set; }
        public string ThesisName { get; set; }
        public string? Description { get; set; }
        public Guid StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public Guid? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? TeacherCode { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public string? ThesisFileUrl { get; set; }
        public string? ThesisFileName { get; set; }
        public ThesisStatus Status { get; set; }
       
    }

}
