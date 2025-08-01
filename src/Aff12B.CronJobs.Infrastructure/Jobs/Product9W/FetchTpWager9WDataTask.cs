using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.DataDomain.Params;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Product9W
{
    public class FetchTpWager9WDataTask(IProduct9WApi licenseeApi,
         IThirdPartyImportRepository _thirdPartyRepo,
         ITrackerService timeTrackerService,
         IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByBatchForMultiRepoTaskBase<TpWager9WModel, CreateTimeTrackerInfo>
            (_thirdPartyRepo, configuration.Get(Constants.ServiceConfigName.PULL_TPWAGER_9W_SETTING), timeTrackerService)
    {
        protected override async Task<IEnumerable<TpWager9WModel>> GetDataAsync(CreateTimeTrackerInfo trackValue, int batchCount)
        {
            var toDate = DateTimeOffset.Now.DateTime;
            return await licenseeApi.GetTpWagerAsync(new BatchCountParam
            {
                CreateTime = trackValue.CreateTime,
                FromDate = trackValue.UpdateTime,
                ToDate = toDate,
                BatchCount = batchCount
            });
        }

        protected override CreateTimeTrackerInfo GetInsertTrackingValue(IEnumerable<TpWager9WModel> data)
        {
            var lastItem = data.LastOrDefault() ?? throw new InvalidOperationException("TpWager9W cannot determine tracking value: data collection is empty.");
            if (!lastItem.CreateTime.HasValue || !lastItem.UpdateTime.HasValue)
            {
                throw new InvalidOperationException("TpWager9W cannot determine tracking value: last item's CreateTime or UpdateTime is null.");
            }

            return new CreateTimeTrackerInfo
            {
                CreateTime = lastItem.CreateTime.Value,
                UpdateTime = lastItem.UpdateTime.Value,
            };
        }

        protected override async Task<CreateTimeTrackerInfo> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var createTimeTrackerInfo = JsonConvert.DeserializeObject<CreateTimeTrackerInfo>(tracker.KeyValue);
            createTimeTrackerInfo.UpdateTime = createTimeTrackerInfo.UpdateTime.AddMinutes(Settings.AdjustmentValue);

            return createTimeTrackerInfo;
        }
    }
}
