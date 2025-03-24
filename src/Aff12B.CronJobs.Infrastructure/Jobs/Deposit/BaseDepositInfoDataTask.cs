using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Deposit
{
    public abstract class BaseDepositInfoDataTask : FetchDataForMultipleRepositoryTaskBase<DepositInfoModel>
    {
        private readonly ILicenseeApi _licenseeApi;
        private readonly ILogger _logger;
        private readonly ITrackerService _trackerService;

        private const int MAX_ROW_COUNT = 2000;
        protected virtual bool IsNeedRealData => false;

        public BaseDepositInfoDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ILogger<BaseDepositInfoDataTask> logger
            , ITrackerService trackerService
            , FetchServiceSetting fetchServiceSetting)
            : base(thirdPartyImportRepository, fetchServiceSetting)
        {
            _licenseeApi = licenseeApi;
            _logger = logger;
            _trackerService = trackerService;
        }

        public override async Task FetchDataAsync()
        {
            var tracker = await GetAdjustmentLastTrackerAsync();
            await FetchDataAsync(tracker.UpdateTime);
        }

        private async Task FetchDataAsync(DateTime fromDate)
        {
            var timeRange = GetApiMaxTimeRange();
            var apiServerNow = DateTimeHelper.GetApiServerDateTime();
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, apiServerNow, timeRange);

            await HandleFetchDataAsync(dateRanges);

            if (IsNeedRealData)
            {
                await HandleFetchRealDataAsync(dateRanges, apiServerNow);
            }
        }

        private async Task HandleFetchDataAsync(IEnumerable<(DateTime FromDate, DateTime ToDate)> dateRanges)
        {
            foreach (var range in dateRanges)
            {
                await HandleImportDataAsync(range.FromDate, range.ToDate);
            }
        }

        protected async Task HandleFetchRealDataAsync(IEnumerable<(DateTime FromDate, DateTime ToDate)> dateRanges, DateTime apiServerNow)
        {
            var lastDateRange = dateRanges.LastOrDefault();

            if (lastDateRange.ToDate != default(DateTime))
            {
                await HandleImportDataAsync(lastDateRange.ToDate, apiServerNow.AddSeconds(-5), false);
            }
        }

        private async Task HandleImportDataAsync(DateTime fromDate, DateTime toDate, bool isNeedImportTracker = true)
        {
            int pageIndex = 0;
            while (true)
            {
                var data = await _licenseeApi.GetDepositInfo(fromDate, toDate, pageIndex, MAX_ROW_COUNT);
                if (!data.Any())
                {
                    break;
                }
                await ImportDataToMultipleRepoAsync(data);
                pageIndex++;

                if (isNeedImportTracker)
                {
                    var trackerItem = new TrackerItem
                    {
                        Id = data.Max(_ => _.DepositId),
                        UpdateTime = data.Max(_ => _.UpdateTime.Value),
                    };
                    await _trackerService.SaveTrackerAsync(TableName,
                        new TrackerInfo
                        {
                            CalledTime = DateTime.Now,
                            KeyValue = JsonConvert.SerializeObject(trackerItem)
                        });
                }

                if (data.Count() < MAX_ROW_COUNT)
                {
                    break;
                }
            }
        }

        private async Task<TrackerItem> GetAdjustmentLastTrackerAsync()
        {
            var tracker = await _trackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var trackerItem = JsonConvert.DeserializeObject<TrackerItem>(tracker.KeyValue);
            trackerItem.UpdateTime = trackerItem.UpdateTime.AddMinutes(Settings.AdjustmentValue);
            return trackerItem;
        }

        protected override string BuildXml(IEnumerable<DepositInfoModel> data)
             => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;

        private TimeSpan GetApiMaxTimeRange()
            => TimeSpan.FromMinutes(Settings.MaxTimeRange);

        private sealed class TrackerItem
        {
            public int Id { get; set; }

            public DateTime UpdateTime { get; set; }
        }
    }
}
