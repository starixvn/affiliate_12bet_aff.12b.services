using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Bonus
{
    public class FetchBonusInfoDataTask(ILicenseeApi licenseeApi,
        IThirdPartyImportRepository thirdPartyRepo,
        ITrackerService timeTrackerService,
        IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByBatchForMultiRepoTaskBase<BonusModel, string>(thirdPartyRepo, configuration.Get(Constants.ServiceConfigName.PULL_BONUS_INFO_SETTING), timeTrackerService)
    {
        protected override int PagingSize => 500;

        protected override Task<IEnumerable<BonusModel>> GetDataAsync(string trackValue, int batchCount)
        {
            var id = Convert.ToInt32(trackValue);
            return licenseeApi.GetBonusInfo(id, batchCount);
        }

        protected override string GetInsertTrackingValue(IEnumerable<BonusModel> data)
            => data.Max(d => d.BonusId).ToString();

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var id = string.IsNullOrWhiteSpace(tracker.KeyValue) ? 0 : int.Parse(tracker.KeyValue);
            var queryId = GetAdjustmentId(id);
            return queryId.ToString();
        }

        private int GetAdjustmentId(int id)
            => id - Settings.AdjustmentValue < 0 ? 0 : id - Settings.AdjustmentValue;
    }
}
