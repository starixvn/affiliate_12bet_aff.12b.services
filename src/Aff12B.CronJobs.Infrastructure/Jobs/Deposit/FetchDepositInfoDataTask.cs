using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Deposit
{
    public class FetchDepositInfoDataTask : FetchDataForMultipleRepositoryTaskBase<DepositInfoModel>
    {
        private readonly ILicenseeApi _licenseeApi;
        private readonly ILogger _logger;
        private readonly ITrackerService _trackerService;

        private const int MAX_ROW_COUNT = 2000;

        public FetchDepositInfoDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ILogger<FetchDepositInfoDataTask> logger
            , ITrackerService trackerService
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, setting.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING))
        {
            _licenseeApi = licenseeApi;
            _logger = logger;
            _trackerService = trackerService;
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public override async Task FetchDataAsync()
        {
            var tracker = await GetAdjustmentLastTrackerAsync();
            await FetchDataAsync(tracker.UpdateTime);
        }

        public async Task FetchDataAsync(DateTime fromDate)
        {
            var timeRange = GetApiMaxTimeRange();
            var apiServerNow = DateTimeHelper.GetApiServerDateTime();
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, apiServerNow, timeRange);
            foreach (var range in dateRanges)
            {
                int pageIndex = 0;
                while (true)
                {
                    var data = await _licenseeApi.GetDepositInfo(range.FromDate, range.ToDate, pageIndex, MAX_ROW_COUNT);
                    if (!data.Any())
                    {
                        break;
                    }
                    await ImportDataToMultipleRepoAsync(data);
                    pageIndex++;

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

                    if (data.Count() < MAX_ROW_COUNT)
                    {
                        break;
                    }
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
