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
    public class FetchTpReport9WDataTask(IProduct9WApi licenseeApi,
         IThirdPartyImportRepository thirdPartyRepo,
         ITrackerService timeTrackerService,
         IOptionsSnapshot<FetchServiceSetting> configuration) : FetchDataByBatchForMultiRepoTaskBase<TpReport9WModel>(thirdPartyRepo, configuration.Get(ServiceConfigName.PULL_TPREPORT_9W_SETTING), timeTrackerService)
    {

        protected override async Task<IEnumerable<TpReport9WModel>> GetDataAsync(string trackValue, int batchCount)
        {
            var fromDate = DateTime.Parse(trackValue);
            var toDate = DateTime.Now;
            return await licenseeApi.GetTpReportAsync(new BaseParam
            {
                FromDate = fromDate,
                ToDate = toDate,
                PageIndex = 0,
                PageSize = batchCount
            });
        }

        protected override string GetInsertTrackingValue(IEnumerable<TpReport9WModel> data)
            => data.Max(d => d.UpdateTime).Value.ToString(Format.DateTimeFormat);

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.KeyValue, new StringToDateTimeComparer());
            var date = string.IsNullOrWhiteSpace(tracker.KeyValue)
                ? throw new ArgumentException($"Not valid DateTime Tracker Value {tracker.KeyValue}")
                : DateTime.Parse(tracker.KeyValue);
            var adjustDate = GetAdjustmentDate(date);
            return adjustDate.ToString(Format.DateTimeFormat);
        }

        private DateTime GetAdjustmentDate(DateTime date)
            => date.AddMinutes(Settings.AdjustmentValue);
    }
}
