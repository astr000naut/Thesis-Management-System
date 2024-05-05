
using System.ComponentModel.DataAnnotations;


namespace TMS.Worker.Core
{
    public class TeacherDto
    {
        [Key]
        public Guid UserId { get; set; }
        public string TeacherCode { get; set; }
        public string TeacherName { get; set; }
        public string FacultyCode { get; set; }
        public string FacultyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public bool BeInstructor { get; set; } = true;
    }
}
