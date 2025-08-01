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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public class FetchBetRecordDataTask : FetchDataForMultipleRepositoryTaskBase<TpBetRecordModel>, IFetchBetRecordDataTask
    {
        private const int API_SERVER_TIME_OFFSET = 0;
        private const int PAGE_SIZE = 2000;

        private readonly ILicenseeApi _licenseeApi;
        private readonly ILogger _logger;
        private readonly ITrackerService _timeTrackerService;

        public FetchBetRecordDataTask(ILicenseeApi licenseeApi,
            IThirdPartyImportRepository _thirdPartyRepo,
            ILogger<FetchBetRecordDataTask> logger,
            ITrackerService timeTrackerService,
            IOptionsSnapshot<FetchServiceSetting> configuration)
            : base(_thirdPartyRepo, configuration.Get(Constants.ServiceConfigName.PULL_BET_RECORD_SETTING))
        {
            _licenseeApi = licenseeApi;
            _logger = logger;
            _timeTrackerService = timeTrackerService;
        }

        public override async Task FetchDataAsync()
        {
            var fromDate = await GetLastTimeFetchDateAsync();
            var toDate = GetAdjustmentApiServerDate();
            await FetchDataAndTrackTimeAsync(fromDate, toDate);
        }

        public async Task FetchDataAsync(DateTime fromDate, DateTime toDate)
        {
            var intervalTime = GetApiMaxTimeRange();
            var timeRanges = DateTimeHelper.GetProportionTimeRange(fromDate, toDate, intervalTime);
            foreach (var range in timeRanges)
            {
                try
                {
                    await FetchAndInsertDataByPageAsync(range.FromDate, range.ToDate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Excetion when fetch BetRecord from {range.FromDate} to {range.ToDate}");
                    throw;
                }
            }
        }

        public async Task FetchLastMonthAsync()
        {
            var now = DateTimeOffset.Now;
            var dayLastMonth = now.AddMonths(-1);
            var firstDayLastMonth = new DateTimeOffset(dayLastMonth.Year, dayLastMonth.Month, 1, 0, 0, 0, now.Offset);
            var firstDayThisMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
            await FetchDataAsync(firstDayLastMonth.DateTime, firstDayThisMonth.DateTime);
        }

        public async Task FetchFromAdjusmentDayAsync(int day)
        {
            var lastFetchDate = await GetLastTimeFetchDateAsync();
            var fromDate = lastFetchDate.AddDays(-day);
            var toDate = GetAdjustmentApiServerDate();
            await FetchDataAsync(fromDate, toDate);
        }


        public async Task FetchAllDay(int adjustHour)
        {
            var lastFetchDate = await GetLastTimeFetchDateAsync();
            var adjustDate = lastFetchDate.AddHours(-adjustHour);
            var fromDate = new DateTimeOffset(adjustDate.Year, adjustDate.Month, adjustDate.Day, 0, 0, 0, DateTimeOffset.Now.Offset);
            var toDate = GetAdjustmentApiServerDate();
            await FetchDataAsync(fromDate.DateTime, toDate);
        }

        private async Task FetchDataAndTrackTimeAsync(DateTime fromDate, DateTime toDate)
        {
            var intervalTime = GetApiMaxTimeRange();
            var timeRanges = DateTimeHelper.GetProportionTimeRange(fromDate, toDate, intervalTime);
            foreach (var range in timeRanges)
            {
                try
                {
                    await FetchAndInsertDataByPageAsync(range.FromDate, range.ToDate);
                    var tracker = new TrackerInfo { CalledTime = DateTimeOffset.Now.DateTime, KeyValue = range.ToDate.ToString(Format.DateTimeFormat, CultureInfo.InvariantCulture) };
                    await _timeTrackerService.SaveTrackerAsync(TableName, tracker);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Excetion {ex.Message} when fetch BetRecord from {range.FromDate} to {range.ToDate}");
                    throw;
                }
            }
        }

        private async Task FetchAndInsertDataByPageAsync(DateTime fromDate, DateTime toDate)
        {
            int pageIndex = 0;
            while (true)
            {
                var data = await _licenseeApi.GetBetRecordAsync(fromDate, toDate, pageIndex, PAGE_SIZE);
                if (data == null || !data.Any())
                {
                    return;
                }

                await ImportDataToMultipleRepoAsync(data);
                pageIndex++;

                if (data.Count() < PAGE_SIZE)
                {
                    return;
                }
            }
        }

        private TimeSpan GetApiMaxTimeRange()
        {
            return TimeSpan.FromMinutes(Settings.MaxTimeRange);
        }

        private async Task<DateTime> GetLastTimeFetchDateAsync()
        {
            var adjustmentMin = Settings.AdjustmentValue;
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var trackingDate = DateTime.ParseExact(tracker.KeyValue, Format.DateTimeFormat, CultureInfo.InvariantCulture);
            var lastFetchDate = trackingDate.AddMinutes(adjustmentMin);

            return lastFetchDate;
        }

        private static DateTime GetAdjustmentApiServerDate()
        {
            var date = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(API_SERVER_TIME_OFFSET));
            return date.DateTime;
        }

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;

        protected override string BuildXml(IEnumerable<TpBetRecordModel> data)
            => XmlHelper.BuildXml(data, "transaction", "trans");
    }
}