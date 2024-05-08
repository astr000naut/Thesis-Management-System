using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository.Param;

namespace TMS.DataLayer.Param
{
    public class TeacherThesisFilterParam
    {
        public string TeacherId { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
        public string KeySearch { get; set; }

    }
}
