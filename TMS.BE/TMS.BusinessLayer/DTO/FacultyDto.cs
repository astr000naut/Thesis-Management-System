using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;

namespace TMS.BusinessLayer.DTO
{
    public class FacultyDto
    {
        [Key]
        public Guid FacultyId { get; set; }
        public string FacultyCode { get; set; }
        public string FacultyName { get; set; }
        public string Description { get; set; }
    }
}
