using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public interface IFetchCustomerDataTask : IFetchDataTask
    {
        Task FetchDataDailyTask();

        Task FetchDataAsync(DateTime fromDate);

        Task FetchFixedDataTask();

        Task FetchDataByDateRangeTask();
    }

    public sealed class FetchCustomerDataTask : FetchDataForMultipleRepositoryTaskBase<CustomerModel>, IFetchCustomerDataTask
    {
        private const int BATCH_COUNT = 2000;
        private const int DATE_RANGE = 1440;

        private readonly ITrackerService _timeTrackerService;
        private readonly ILicenseeApi _liceseeApi;

        public FetchCustomerDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ITrackerService timeTrackerService
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, setting.Get(ServiceConfigName.PULL_CUSTOMER_SETTING))
        {
            _timeTrackerService = timeTrackerService ?? throw new ArgumentNullException(nameof(timeTrackerService));
            _liceseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public override async Task FetchDataAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var fromDate = GetAdjustmentTime(DateTime.ParseExact(tracker.KeyValue, Format.DateTimeFormat, null));
            await FetchDataAsync(fromDate);
        }

        public async Task FetchDataAsync(DateTime fromDate)
        {
            var timeRange = GetApiMaxTimeRange();
            var apiServerNow = DateTimeHelper.GetApiServerDateTime();
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, apiServerNow, timeRange);

            await HandleFetchDataAsync(dateRanges);
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task FetchDataByDateRangeTask()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var fromDate = GetAdjustmentTime(DateTime.ParseExact(tracker.KeyValue, Format.DateTimeFormat, null));

            var now = DateTimeHelper.GetApiServerDateTime();
            var maxTimeRange = TimeSpan.FromMinutes(DATE_RANGE);
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, now, maxTimeRange);

            await HandleFetchDataAsync(dateRanges);
        }

        public async Task HandleFetchDataAsync(IEnumerable<(DateTime FromDate, DateTime ToDate)> dateRanges)
        {
            foreach (var range in dateRanges)
            {
                int pageIndex = 0;
                while (true)
                {
                    var data = await _liceseeApi.GetCustomersAsync(range.FromDate, range.ToDate, pageIndex, BATCH_COUNT);
                    if (!data.Any())
                    {
                        break;
                    }
                    await ImportDataToMultipleRepoAsync(data);
                    pageIndex++;
                    if (data.Count() < BATCH_COUNT)
                    {
                        break;
                    }
                }
                var tracker = new TrackerInfo { CalledTime = DateTime.Now, KeyValue = range.ToDate.ToString(Format.DateTimeFormat) };
                await _timeTrackerService.SaveTrackerAsync(TableName, tracker);
            }
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task FetchDataDailyTask()
        {
            var now = DateTimeHelper.GetApiServerDateTime();
            var adjustmentDay = now.AddDays(-1);
            var fromDate = new DateTime(adjustmentDay.Year, adjustmentDay.Month, adjustmentDay.Day, 0, 0, 0);
            var maxTimeRange = GetApiMaxTimeRange();
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, now, maxTimeRange);
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

        public async Task FetchFixedDataTask()
        {
            var data = await _liceseeApi.GetFixedCustomers();
            await ImportDataToMultipleRepoAsync(data);
        }

        private DateTime GetAdjustmentTime(DateTime fromDate)
            => fromDate.AddMinutes(Settings.AdjustmentValue);


        protected override string BuildXml(IEnumerable<CustomerModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;

        private TimeSpan GetApiMaxTimeRange()
            => TimeSpan.FromMinutes(Settings.MaxTimeRange);
    }
}