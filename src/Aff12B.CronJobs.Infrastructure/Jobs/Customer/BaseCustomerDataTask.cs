using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Customer
{
    public abstract class BaseCustomerDataTask : FetchDataForMultipleRepositoryTaskBase<CustomerModel>
    {
        protected const int BATCH_COUNT = 2000;
        protected virtual bool IsNeedRealData => false;

        private readonly ITrackerService _timeTrackerService;
        protected readonly ILicenseeApi _liceseeApi;

        public BaseCustomerDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ITrackerService timeTrackerService
            , FetchServiceSetting fetchServiceSetting)
            : base(thirdPartyImportRepository, fetchServiceSetting)
        {
            _timeTrackerService = timeTrackerService ?? throw new ArgumentNullException(nameof(timeTrackerService));
            _liceseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
        }

        public override async Task FetchDataAsync()
        {
            var maxTimeRange = GetApiMaxTimeRange();

            await FetchDataAsync(maxTimeRange);
        }

        protected async Task FetchDataAsync(TimeSpan maxTimeRange)
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var fromDate = GetAdjustmentTime(DateTime.ParseExact(tracker.KeyValue, Format.DateTimeFormat, null));

            var apiServerNow = DateTimeHelper.GetApiServerDateTime();
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, apiServerNow, maxTimeRange);

            await HandleFetchDataAsync(dateRanges);

            if (IsNeedRealData)
            {
                await HandleFetchRealDataAsync(dateRanges, apiServerNow);
            }    
        }

        protected async Task HandleFetchDataAsync(IEnumerable<(DateTime FromDate, DateTime ToDate)> dateRanges)
        {
            foreach (var range in dateRanges)
            {
                await HandleImportDataAsync(range.FromDate, range.ToDate);

                var tracker = new TrackerInfo { CalledTime = DateTime.Now, KeyValue = range.ToDate.ToString(Format.DateTimeFormat) };
                await _timeTrackerService.SaveTrackerAsync(TableName, tracker);
            }
        }

        protected async Task HandleFetchRealDataAsync(IEnumerable<(DateTime FromDate, DateTime ToDate)> dateRanges, DateTime apiServerNow)
        {
            var lastDateRange = dateRanges.LastOrDefault();

            if(lastDateRange.ToDate != default(DateTime))
            {
                await HandleImportDataAsync(lastDateRange.ToDate, apiServerNow.AddSeconds(-5));
            }
        }

        private async Task HandleImportDataAsync(DateTime fromDate, DateTime toDate)
        {
            int pageIndex = 0;
            while (true)
            {
                var data = await _liceseeApi.GetCustomersAsync(fromDate, toDate, pageIndex, BATCH_COUNT);
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
        }

        private DateTime GetAdjustmentTime(DateTime fromDate)
            => fromDate.AddMinutes(Settings.AdjustmentValue);

        protected override string BuildXml(IEnumerable<CustomerModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;

        protected TimeSpan GetApiMaxTimeRange()
            => TimeSpan.FromMinutes(Settings.MaxTimeRange);
    }
}
