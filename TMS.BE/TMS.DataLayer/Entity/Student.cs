﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common.Attribute;

namespace TMS.DataLayer.Entity
{
    [Common.Attribute.Table("students", "view_students")]
    public class Student
    {
        [Key]
        public Guid UserId { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string Class { get; set; }
        public string Major { get; set; }
        public string FacultyCode { get; set; }
        [NotMapped]
        public string FacultyName { get; set; }
        public string GPA { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Description { get; set; }

    }
}
