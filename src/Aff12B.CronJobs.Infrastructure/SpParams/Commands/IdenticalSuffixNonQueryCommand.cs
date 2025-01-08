using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.SpParams.Commands
{
    public abstract class IdenticalSuffixNonQueryCommand(string prefixProvider = null) : IdenticalSurfixCriteriaBase(prefixProvider), INonQueryCommand
    {
    }
}
