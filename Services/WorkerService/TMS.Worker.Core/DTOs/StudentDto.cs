
using System.ComponentModel.DataAnnotations;

namespace TMS.Worker.Core
{
    public class StudentDto
    {
        [Key]
        public Guid UserId { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string? Class { get; set; }
        public string? Major { get; set; }
        public string? FacultyCode { get; set; }
        public string? FacultyName { get; set; }
        public string? GPA { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
    }
}
