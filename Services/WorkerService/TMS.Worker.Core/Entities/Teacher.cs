
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Worker.Core
{
    [Attributes.Table("teachers", "view_teachers")]
    public class Teacher
    {
        [Key]
        public Guid UserId { get; set; }
        public string TeacherCode { get; set; }
        public string TeacherName { get; set; }
        public string FacultyCode { get; set; }
        [NotMapped]
        public string FacultyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public bool BeInstructor { get; set; } = true;

    }
}
