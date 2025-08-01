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
using System.Globalization;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Log
{
    public class FetchRebateMLogDataTask : FetchDataForMultipleRepositoryTaskBase<RebateMLogModel>
    {
        private ILicenseeApi Api { get; }
        private ILogger Logger { get; }
        private ITrackerService TrackerService { get; }

        private const int API_SERVER_TIME_OFFSET = 0;

        public FetchRebateMLogDataTask(ILicenseeApi licenseeApi,
            IThirdPartyImportRepository _thirdPartyRepo,
            ILogger<FetchRebateMLogDataTask> logger,
            ITrackerService timeTrackerService,
            IOptionsSnapshot<FetchServiceSetting> configuration)
            : base(_thirdPartyRepo, configuration.Get(ServiceConfigName.PULL_REBATEMLOG_SETTING))
        {
            Api = licenseeApi;
            Logger = logger;
            TrackerService = timeTrackerService;
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public override async Task FetchDataAsync()
        {
            var tracker = await TrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var fromDate = DateTime.Parse(tracker.KeyValue, CultureInfo.InvariantCulture);
            var toDate = GetAdjustmentTime();
            await FetchDataAndTrackTimeAsync(fromDate, toDate);
        }

        private async Task FetchDataAndTrackTimeAsync(DateTime fromDate, DateTime toDate)
        {
            var intervalTime = GetApiMaxTimeRange();
            var timeRanges = DateTimeHelper.GetProportionTimeRange(fromDate, toDate, intervalTime);
            foreach (var range in timeRanges)
            {
                try
                {
                    await FetchAndInsertDataAsync(range.FromDate, range.ToDate);
                    var tracker = new TrackerInfo { CalledTime = DateTimeOffset.Now.DateTime, KeyValue = range.ToDate.ToString(Format.DateTimeFormat) };
                    await TrackerService.SaveTrackerAsync(TableName, tracker);
                }
                catch
                {
                    Logger.LogError($"Excetion when fetch {TableName} from {range.FromDate} to {range.ToDate}");
                    throw;
                }
            }
        }

        private async Task FetchAndInsertDataAsync(DateTime fromDate, DateTime toDate)
        {
            var datas = await Api.GetRebateMLogs(fromDate, toDate);
            await ImportDataToMultipleRepoAsync(datas);
        }

        private DateTime GetAdjustmentTime() => GetAdjustmentApiServerDate(new TimeSpan(0, Settings.AdjustmentValue, 0));

        protected override string BuildXml(IEnumerable<RebateMLogModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;

        private TimeSpan GetApiMaxTimeRange() => TimeSpan.FromMinutes(Settings.MaxTimeRange);

        private static DateTime GetAdjustmentApiServerDate(TimeSpan ajustmentTime)
        {
            var date = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(API_SERVER_TIME_OFFSET));
            var adjustDate = date.Add(ajustmentTime);
            return adjustDate.DateTime;
        }
    }
}