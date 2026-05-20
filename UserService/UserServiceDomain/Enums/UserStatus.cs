using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceDomain.Enums
{
    public enum UserStatus
    {
        Active,
        InActive,
        PendingApproval //za trenere sve dok ih admin ne odobri.
    }
}
