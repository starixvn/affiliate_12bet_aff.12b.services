using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using ONEbook.UM.Services;

namespace Aff12B.CronJobs.Host.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class UMHangfireFilterAttribute(IMaintenanceService maintenanceService, ILogger<UMHangfireFilterAttribute> logger) : JobFilterAttribute, IClientFilter, IServerFilter
    {
        public void OnCreating(CreatingContext context)
        {
            if (maintenanceService.IsUnderMaintenance())
            {
                logger.LogInformation("Aff12B.CronJobs Is Under Maintenance!");
                context.Canceled = true;
            }
        }

        public void OnCreated(CreatedContext context)
        {
            //Nothing to do
        }

        public void OnPerforming(PerformingContext context)
        {
            if (maintenanceService.IsUnderMaintenance())
            {
                logger.LogInformation("Aff12B.CronJobs Is Under Maintenance!");
                context.Canceled = true;
            }
        }

        public void OnPerformed(PerformedContext context)
        {
            //Nothing to do
        }
    }

}
