using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aff12B.CronJobs.Core.Domain
{
    public interface IPagingInfo
    {
        int PageIndex { get; }

        int PageSize { get; }
    }
}
