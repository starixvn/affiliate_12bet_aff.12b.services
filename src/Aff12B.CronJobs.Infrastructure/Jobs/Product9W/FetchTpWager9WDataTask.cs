using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.DataDomain.Params;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Comparer;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Product9W
{
    public class FetchTpWager9WDataTask(IProduct9WApi licenseeApi,
         IThirdPartyImportRepository _thirdPartyRepo,
         ITrackerService timeTrackerService,
         IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByBatchForMultiRepoTaskBase<TpWager9WModel>(_thirdPartyRepo, configuration.Get(Constants.ServiceConfigName.PULL_TPWAGER_9W_SETTING), timeTrackerService)
    {
        protected override async Task<IEnumerable<TpWager9WModel>> GetDataAsync(string trackValue, int batchCount)
        {
            var fromDate = DateTime.Parse(trackValue);
            var toDate = DateTime.Now;
            return await licenseeApi.GetTpWagerAsync(new BaseParam
            {
                FromDate = fromDate,
                ToDate = toDate,
                PageIndex = 0,
                PageSize = batchCount
            });
        }

        protected override string GetInsertTrackingValue(IEnumerable<TpWager9WModel> data)
            => data.Max(d => d.UpdateTime).Value.ToString(Format.DateTimeFormat);

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.KeyValue, new StringToDateTimeComparer());
            var date = string.IsNullOrWhiteSpace(tracker.KeyValue)
                ? throw new ArgumentException($"Not valid DateTime Tracker Value {tracker.KeyValue} of table {TableName}")
                : DateTime.Parse(tracker.KeyValue);
            var adjustDate = GetAdjustmentDate(date);
            return adjustDate.ToString(Format.DateTimeFormat);
        }

        private DateTime GetAdjustmentDate(DateTime date)
            => date.AddMinutes(Settings.AdjustmentValue);
    }
}
