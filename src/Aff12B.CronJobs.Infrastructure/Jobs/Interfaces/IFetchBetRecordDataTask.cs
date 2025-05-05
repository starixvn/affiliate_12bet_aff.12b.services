using Hangfire;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Interfaces
{
    public interface IFetchBetRecordDataTask : IFetchDataTask
    {
        [DisableConcurrentExecution(120)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete, Attempts = 1)]
        Task FetchLastMonthAsync();

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete, Attempts = 1)]
        Task FetchFromAdjusmentDayAsync(int day);

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete, Attempts = 1)]
        Task FetchAllDay(int adjustHour);
    }
}
