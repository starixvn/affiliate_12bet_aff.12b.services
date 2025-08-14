using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;

namespace Aff12B.CronJobs.Infrastructure.Jobs.Base
{
    /// <summary>
    /// Base class for tasks that fetch data using DateTime tracking.
    /// Provides common implementation for DateTime-based tracking operations.
    /// </summary>
    /// <typeparam name="TData">The type of data to fetch</typeparam>
    public abstract class FetchDataByDateTimeTrackerForMultiRepoTaskBase<TData> : FetchDataByBatchForMultiRepoTaskBase<TData, string>
        where TData : class, IHasUpdateTime
    {
        protected FetchDataByDateTimeTrackerForMultiRepoTaskBase(
            IThirdPartyImportRepository thirdPartyImportRepository,
            FetchServiceSetting setting,
            ITrackerService trackerService)
            : base(thirdPartyImportRepository, setting, trackerService)
        {
        }

        protected override string GetInsertTrackingValue(IEnumerable<TData> data)
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