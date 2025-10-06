using Hangfire;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Interfaces
{
    public interface IUpdateDataCacheTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task UpdateDataCacheAsync();
    }
}