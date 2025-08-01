using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public class FetchWithdrawalInfoDataTask(
        IThirdPartyImportRepository thirdPartyImportRepository,
        ITrackerService timeTrackerService,
        ILicenseeApi licenseeApi,
        IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByBatchForMultiRepoTaskBase<WithdrawalInfoModel, string>
            (thirdPartyImportRepository, configuration.Get(Constants.ServiceConfigName.PULL_WITHDRAWAL_INFO), timeTrackerService)
    {
        protected override Task<IEnumerable<WithdrawalInfoModel>> GetDataAsync(string trackValue, int batchCount)
            => licenseeApi.GetWithdrawalInfo(Convert.ToDateTime(trackValue), batchCount);

        protected override string GetInsertTrackingValue(IEnumerable<WithdrawalInfoModel> data)
        {
            var trackValue = data.Max(d => d.UpdateTime);

            if (trackValue.HasValue)
            {
                return trackValue.Value.ToString(Format.DateTimeFormat);
            }
            throw new ArgumentException($"Can not insert nullable DateTime Tracker Value of table {TableName}");
        }

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var date = string.IsNullOrWhiteSpace(tracker.KeyValue)
                ? throw new ArgumentException($"Not valid DateTime Tracker Value {tracker.KeyValue} of table {TableName}")
                : Convert.ToDateTime(tracker.KeyValue);
            var adjustDateTime = GetAdjustmentTime(date);
            return adjustDateTime.ToString(Format.DateTimeFormat);
        }

        private DateTime GetAdjustmentTime(DateTime date) => date.AddMinutes(Settings.AdjustmentValue);
    }
}