using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.RetryJob
{
    public class ReInsertAffInfosDataTask : IRetryDataTask
    {
        private readonly IDataTrackerService _trackerService;
        private readonly ILicenseeApi _liceseeApi;
        private readonly ILogger _logger;

        private static readonly IEnumerable<int> ListPullStatusId = new[] { DataTrackerStatus.InProcess, DataTrackerStatus.Failure };

        private FetchServiceSetting Setting { get; }

        public ReInsertAffInfosDataTask(IDataTrackerService dataTrackerService,
            ILicenseeApi licenseeApi,
            IOptionsSnapshot<FetchServiceSetting> configuration,
            ILogger<ReInsertAffInfosDataTask> logger
            )
        {
            _trackerService = dataTrackerService;
            _liceseeApi = licenseeApi;
            _logger = logger;
            Setting = configuration.Get(ServiceConfigName.INSERT_AFFILIATE_SETTING);
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task HandleAsync()
        {
            var prefixProviders = Setting.SupportedPrefix;
            var dataTrackerItems = await _trackerService.FetchTrackerAsync(
                prefixProviders,
                new[] { DataTrackerType.InsertAffInfos },
                ListPullStatusId
            );

            foreach (var item in dataTrackerItems)
            {
                try
                {
                    var affInfos = JsonConvert.DeserializeObject<IEnumerable<AffInfo>>(item.PushDataInfo);
                    var result = await _liceseeApi.InsertAffInfos(affInfos);

                    if (result.IsSuccess)
                    {
                        await _trackerService.UpdateTrackerStatusAsync(item, DataTrackerStatus.Success);
                    }
                    else
                    {
                        _logger.LogError($"Reinsert aff infos to {item.PrefixSiteName} fail - {result.ErrMsg}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Reinsert aff infos to {item.PrefixSiteName} DB throw exception - {ex}");
                }
            }
        }
    }
}
