using Hangfire.Dashboard;

namespace Aff12B.CronJobs.Host.Filters
{
    public class AllowedAnonymousAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }

}
