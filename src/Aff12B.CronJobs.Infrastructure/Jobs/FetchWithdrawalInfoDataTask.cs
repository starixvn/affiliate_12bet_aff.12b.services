using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{

    public class FetchWithdrawalInfoDataTask(IThirdPartyImportRepository thirdPartyImportRepository,
        ITrackerService timeTrackerService,
        ILogger<FetchWithdrawalInfoDataTask> logger,
        ILicenseeApi licenseeApi,
        IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByBatchForMultiRepoTaskBase<WithdrawalInfoModel>(thirdPartyImportRepository, configuration.Get(Constants.ServiceConfigName.PULL_WITHDRAWAL_INFO), timeTrackerService)
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";
        private const int BatchCount = 2000;

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "<Pending>")]
        public override async Task FetchDataAsync()
        {
            try
            {
                var queryValue = await GetLastTrackingValueAsync();
                var fromDate = Convert.ToDateTime(queryValue);

                var data = await GetDataAsync(fromDate, BatchCount);

                await ImportDataByBatchAsync(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        protected override async Task ImportDataByBatchAsync(IEnumerable<WithdrawalInfoModel> data)
        {
            var importData = data;
            while (importData != null && importData.Any())
            {
                await ImportDataToMultipleRepoAsync(importData);
                var trackValue = GetInsertTrackingValue(importData);
                var fromTime = Convert.ToDateTime(trackValue);

                await _timeTrackerService.SaveTrackerAsync(TableName,
                    new TrackerInfo
                    {
                        CalledTime = DateTime.Now,
                        KeyValue = trackValue
                    });

                if (importData.Count() == BatchCount)
                {
                    importData = await GetDataAsync(fromTime, BatchCount);
                }
                else
                {
                    break;
                }
            }
        }

        protected override Task<IEnumerable<WithdrawalInfoModel>> GetDataAsync(string trackValue, int batchCount)
        {
            throw new NotImplementedException();
        }

        private Task<IEnumerable<WithdrawalInfoModel>> GetDataAsync(DateTime updateTime, int batchCount)
            => licenseeApi.GetWithdrawalInfo(updateTime, batchCount);

        protected override string GetInsertTrackingValue(IEnumerable<WithdrawalInfoModel> data)
        {
            var trackValue = data.Max(d => d.UpdateTime);

            if (trackValue.HasValue)
            {
                return trackValue.Value.ToString();
            }
            throw new ArgumentException($"Can not insert nullable DateTime Tracker Value of table {TableName}");
        }

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var date = string.IsNullOrWhiteSpace(tracker.KeyValue)
                ? throw new ArgumentException($"Not valid DateTime Tracker Value {tracker.KeyValue} of table {TableName}")
                : Convert.ToDateTime(tracker.KeyValue);
            var query = GetAdjustmentTime(date);
            return query.ToString(DateTimeFormat);
        }

        private DateTime GetAdjustmentTime(DateTime date) => date.AddMinutes(Settings.AdjustmentValue);
    }
}