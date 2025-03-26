using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Customer
{
    public interface IFetchCustomerDataTask : IFetchDataTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataDailyTask();

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataByDateRangeTask();
    }

    public sealed class FetchCustomerDataTask : BaseCustomerDataTask, IFetchCustomerDataTask
    {
        private const int DATE_RANGE = 1440;

        public FetchCustomerDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ITrackerService timeTrackerService
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, licenseeApi, timeTrackerService, setting.Get(ServiceConfigName.PULL_CUSTOMER_SETTING))
        { }

        public async Task FetchDataByDateRangeTask()
        {
            var maxTimeRange = TimeSpan.FromMinutes(DATE_RANGE);

            await FetchDataAsync(maxTimeRange);
        }

        public async Task FetchDataDailyTask()
        {
            var maxTimeRange = GetApiMaxTimeRange();
            var apiServerNow = DateTimeHelper.GetApiServerDateTime();
            var adjustmentDay = apiServerNow.AddDays(-1);
            var fromDate = new DateTime(adjustmentDay.Year, adjustmentDay.Month, adjustmentDay.Day, 0, 0, 0);
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, apiServerNow, maxTimeRange);

            foreach (var range in dateRanges)
            {
                var pageIndex = 0;
                while (true)
                {
                    var data = await _liceseeApi.GetCustomersAsync(range.FromDate, range.ToDate, pageIndex, BATCH_COUNT);
                    await ImportDataToMultipleRepoAsync(data);
                    pageIndex++;
                    if (data.Count() < BATCH_COUNT)
                    {
                        break;
                    }
                }
            }

        }
    }
}