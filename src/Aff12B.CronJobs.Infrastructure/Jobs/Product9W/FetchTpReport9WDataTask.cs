using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.DataDomain.Params;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Product9W
{
    public class FetchTpReport9WDataTask(IProduct9WApi licenseeApi,
         IThirdPartyImportRepository thirdPartyRepo,
         ITrackerService timeTrackerService,
         IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByBatchForMultiRepoTaskBase<TpReport9WModel, SQTrackerInfo>
            (thirdPartyRepo, configuration.Get(ServiceConfigName.PULL_TPREPORT_9W_SETTING), timeTrackerService)
    {
        protected override async Task<IEnumerable<TpReport9WModel>> GetDataAsync(SQTrackerInfo trackValue, int batchCount)
        {
            var toDate = DateTimeOffset.Now.DateTime;

            return await licenseeApi.GetTpReportAsync(new BatchCountParam
            {
                SQ = trackValue.SQ,
                FromDate = trackValue.UpdateTime,
                ToDate = toDate,
                BatchCount = batchCount
            });
        }

        protected override SQTrackerInfo GetInsertTrackingValue(IEnumerable<TpReport9WModel> data)
        {
            var lastItem = data.LastOrDefault() ?? throw new InvalidOperationException("TpReport9W cannot determine tracking value: data collection is empty.");
            if (!lastItem.UpdateTime.HasValue)
            {
                throw new InvalidOperationException("TpReport9W cannot determine tracking value: last item's UpdateTime is null.");
            }

            return new SQTrackerInfo
            {
                SQ = lastItem.SQ,
                UpdateTime = lastItem.UpdateTime.Value,
            };
        }

        protected override async Task<SQTrackerInfo> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var sqTrackerInfo = JsonConvert.DeserializeObject<SQTrackerInfo>(tracker.KeyValue);
            sqTrackerInfo.UpdateTime = sqTrackerInfo.UpdateTime.AddMinutes(Settings.AdjustmentValue);

            return sqTrackerInfo;
        }
    }
}
