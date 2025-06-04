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
         IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByBatchForMultiRepoTaskBase<TpReport9WModel>(thirdPartyRepo, configuration.Get(ServiceConfigName.PULL_TPREPORT_9W_SETTING), timeTrackerService)
    {
        protected override async Task<IEnumerable<TpReport9WModel>> GetDataAsync(string trackValue, int batchCount)
        {
            var trackerInfo = JsonConvert.DeserializeObject<SQTrackerInfo>(trackValue);
            var toDate = DateTime.Now;

            return await licenseeApi.GetTpReportAsync(new BaseParam
            {
                SQ = trackerInfo.SQ,
                FromDate = trackerInfo.UpdateTime,
                ToDate = toDate,
                PageIndex = 0,
                PageSize = batchCount
            });
        }

        protected override string GetInsertTrackingValue(IEnumerable<TpReport9WModel> data)
        {
            var sqTrackerInfo = new SQTrackerInfo
            {
                SQ = data.Max(_ => _.SQ),
                UpdateTime = data.Max(_ => _.UpdateTime.Value),
            };

            return JsonConvert.SerializeObject(sqTrackerInfo);
        }

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var sqTrackerInfo = JsonConvert.DeserializeObject<SQTrackerInfo>(tracker.KeyValue);
            sqTrackerInfo.UpdateTime = sqTrackerInfo.UpdateTime.AddMinutes(Settings.AdjustmentValue);

            return JsonConvert.SerializeObject(sqTrackerInfo);
        }
    }
}
