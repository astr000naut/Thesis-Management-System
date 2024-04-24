using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DataLayer.Enum
{
    public enum ThesisStatus
    {
        WaitingForApproval = 0,
        NotApproved = 1,
        Approved = 2,
        Rejected = 3,
        Finished = 4
    }
}
