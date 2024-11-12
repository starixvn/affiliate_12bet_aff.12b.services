using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Exceptions;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Services
{
    public class TimeTrackerService : ITrackerService
    {
        private readonly ITrackerRepository _timeTrackerRepository;
        private readonly ILogger _logger;

        private readonly TimeTrackerSetting Setting;

        public TimeTrackerService(ITrackerRepository timeTrackerRepository
            , IOptionsSnapshot<TimeTrackerSetting> optionsSnapshot
            , ILogger<TimeTrackerService> logger)
        {
            _timeTrackerRepository = timeTrackerRepository ?? throw new ArgumentNullException(nameof(timeTrackerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Setting = optionsSnapshot?.Value ?? throw new ArgumentNullException($"{nameof(optionsSnapshot)}.Value");
        }

        public async Task<TrackerInfo> GetTrackerAsync<TKey>(string source, Func<TrackerInfo, TKey> minKeySelector)
        {
            var listTracker = await GetListTrackerAsync(source);
            if (!listTracker.Any())
                throw new InvalidOperationException("Can't fetch any track time");
            return listTracker.OrderBy(minKeySelector).FirstOrDefault();
        }

        public async Task<TrackerInfo> GetTrackerAsync<TKey>(string source, Func<TrackerInfo, TKey> minKeySelector, IComparer<TKey> comparer)
        {
            var listTracker = await GetListTrackerAsync(source);
            if (!listTracker.Any())
                throw new InvalidOperationException("Can't fetch any track time");
            return listTracker.OrderBy(minKeySelector, comparer).FirstOrDefault();
        }

        public async Task SaveTrackerAsync(string source, TrackerInfo tracker)
        {
            var supportedPrefix = Setting.SourcePrefix;
            foreach (var p in supportedPrefix)
            {
                var param = new InsertImportTrackerCommand(p)
                {
                    CalledTime = tracker.CalledTime,
                    KeyValue = tracker.KeyValue,
                    IsReset = tracker.IsReset,
                    DataSource = source,
                    ErrCode = SqlResponseCode.Success,
                    ErrMsg = string.Empty
                };
                await _timeTrackerRepository.ImportTimeTrackerAsync(param);
                if (param.ErrCode != SqlResponseCode.Success)
                {
                    throw new ImporterException(param.ErrCode, param.ErrMsg);
                }
            }
        }

        private async Task<IEnumerable<TrackerInfo>> GetListTrackerAsync(string source)
        {
            var lstTracker = new List<TrackerInfo>();
            var supportedPrefix = Setting.SourcePrefix;
            foreach (var prefix in supportedPrefix)
            {
                var param = new GetLastedImportTimeCriteria(prefix)
                {
                    DataSource = source
                };
                try
                {
                    var trackerInfo = await _timeTrackerRepository.GetLastedTrackerAsync(param);
                    if (trackerInfo == null)
                    {
                        _logger.LogWarning($"Can't get track time from {source} of DB {prefix}");
                        continue;
                    }
                    lstTracker.Add(trackerInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ErrorMessageHelper.GenerateMessage(ex));
                }
            }
            return lstTracker;
        }
    }
}