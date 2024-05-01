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
        ApprovedGuiding = 1,
        RejectedGuiding = 2,
        ApprovedTitle = 3,
        Finished = 4
    }
}
