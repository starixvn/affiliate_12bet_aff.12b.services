using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.SpParams.Commands
{
    public abstract class IdenticalSuffixNonQueryCommand : IdenticalSurfixCriteriaBase, INonQueryCommand
    {
        protected IdenticalSuffixNonQueryCommand(string prefixProvider = null) : base(prefixProvider)
        {
        }
    }
}
