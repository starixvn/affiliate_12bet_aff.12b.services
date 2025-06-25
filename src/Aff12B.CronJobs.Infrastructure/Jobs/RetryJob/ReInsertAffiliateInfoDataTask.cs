using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Aff12B.Common.DataDomain.Params;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.RetryJob
{
    public class ReInsertAffiliateInfoDataTask : IRetryDataTask
    {
        private readonly IDataTrackerService _trackerService;
        private readonly ILicenseeApi _liceseeApi;
        private readonly ILogger _logger;

        private static readonly IEnumerable<int> ListPullStatusId = new int[] { DataTrackerStatus.InProcess, DataTrackerStatus.Failure };

        private FetchServiceSetting Setting { get; }

        public ReInsertAffiliateInfoDataTask(IDataTrackerService dataTrackerService,
            ILicenseeApi licenseeApi,
            IOptionsSnapshot<FetchServiceSetting> configuration,
            ILogger<ReInsertAffiliateInfoDataTask> logger
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
                new int[] { DataTrackerType.InsertAffiliate },
                ListPullStatusId
            );

            foreach (var item in dataTrackerItems)
            {
                var affiliateInfo = JsonConvert.DeserializeObject<AffiliateParam>(item.PushDataInfo);
                var result = await _liceseeApi.InsertAffiliateInfo(affiliateInfo);
                if (result.IsSuccess)
                {
                    await _trackerService.UpdateTrackerStatusAsync(item, DataTrackerStatus.Success);
                }
                else
                {
                    _logger.LogError($"Insert affiliate to 12B DB fail - {result.ErrMsg}");
                }
            }
        }
    }
}
