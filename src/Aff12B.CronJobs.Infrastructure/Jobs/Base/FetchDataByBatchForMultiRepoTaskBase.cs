using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Hangfire;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Base
{
    public abstract class FetchDataByBatchForMultiRepoTaskBase<TData, TTracker>(IThirdPartyImportRepository thirdPartyImportRepository, FetchServiceSetting setting, ITrackerService trackerService)
        : FetchDataForMultipleRepositoryTaskBase<TData>(thirdPartyImportRepository, setting)
        where TData : class
    {
        protected const int MAX_ROW_COUNT = 2000;

        protected readonly ITrackerService _timeTrackerService = trackerService;

        protected override int PagingSize => MAX_ROW_COUNT;

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public override async Task FetchDataAsync()
        {
            var queryValue = await GetLastTrackingValueAsync();
            var data = await GetDataAsync(queryValue, PagingSize);

            await ImportDataByBatchAsync(data);
        }

        protected virtual async Task ImportDataByBatchAsync(IEnumerable<TData> data)
        {
            var importData = data;
            while (importData != null && importData.Any())
            {
                await ImportDataToMultipleRepoAsync(importData);
                var trackValue = GetInsertTrackingValue(importData);
                var keyValue = trackValue is string ? trackValue as string : JsonConvert.SerializeObject(trackValue);

                await _timeTrackerService.SaveTrackerAsync(TableName,
                    new TrackerInfo
                    {
                        CalledTime = DateTimeOffset.Now.DateTime,
                        KeyValue = keyValue
                    });

                if (importData.Count() == PagingSize)
                {
                    importData = await GetDataAsync(trackValue, PagingSize);
                }
                else
                {
                    break;
                }
            }
        }

        protected abstract Task<IEnumerable<TData>> GetDataAsync(TTracker trackValue, int batchCount);

        protected abstract TTracker GetInsertTrackingValue(IEnumerable<TData> data);

        protected abstract Task<TTracker> GetLastTrackingValueAsync();

        protected override string BuildXml(IEnumerable<TData> data)
             => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;
    }
}
