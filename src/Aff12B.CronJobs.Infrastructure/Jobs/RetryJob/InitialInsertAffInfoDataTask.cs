using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aff12B.CronJobs.Infrastructure.Jobs.RetryJob
{
    public class InitialInsertAffInfoDataTask : IRetryDataTask
    {
        private const int PAGE_INDEX_DEFAULT = 1;
        private const int PAGE_SIZE_DEFAULT = 1000;
        private const int AFF_MARKETING_CATEGORY_ID = 2;

        private readonly IDataTrackerService _trackerService;
        private readonly IAffInfoRepository _affInfoRepository;
        private readonly ILicenseeApi _liceseeApi;
        private readonly ILogger _logger;

        private FetchServiceSetting FetchServiceSetting { get; }
        private readonly LicenseeSiteSetting _licenseeSiteSetting;

        private IEnumerable<string> SupportedSites => _licenseeSiteSetting?.SupportedSites;

        public InitialInsertAffInfoDataTask(
            IDataTrackerService dataTrackerService,
            IAffInfoRepository affInfoRepository,
            ILicenseeApi licenseeApi,
            IOptionsSnapshot<FetchServiceSetting> configuration,
            IOptions<LicenseeSiteSetting> config,
            ILogger<InitialInsertAffInfoDataTask> logger
            )
        {
            _trackerService = dataTrackerService;
            _affInfoRepository = affInfoRepository;
            _liceseeApi = licenseeApi;
            _logger = logger;
            FetchServiceSetting = configuration.Get(ServiceConfigName.INITIAL_INSERT_AFFILIATE_SETTING);
            _licenseeSiteSetting = config.Value;
        }


        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 0)]
        public async Task HandleAsync()
        {
            if (SupportedSites == null || !SupportedSites.Any())
            {
                return;
            }

            _logger.LogWarning($"Start importing Aff Infos to DB - {DateTimeOffset.Now.ToString(Format.DateTimeFormat)}");

            foreach (var site in SupportedSites)
            {
                var sitePrefixProvider = _licenseeSiteSetting.GetCodePrefix(site);

                var affInfos = await GetDataAsync(sitePrefixProvider);
                await ImportDataByBatchAsync(affInfos, sitePrefixProvider);
            }

            _logger.LogWarning($"Complete importing Aff Infos to DB - {DateTimeOffset.Now.ToString(Format.DateTimeFormat)}");
        }

        private async Task ImportDataByBatchAsync(IEnumerable<AffInfo> data, string sitePrefixProvider)
        {
            var pageIndex = PAGE_INDEX_DEFAULT;
            var importData = data;

            while (importData != null && importData.Any())
            {
                var validAffs = importData.Where(a => !(string.IsNullOrEmpty(a.AffCode) || (a.AffCategoryId != AFF_MARKETING_CATEGORY_ID && a.Player_CustId == 0)));

                if (validAffs.Any())
                {
                    var prefixProviders = FetchServiceSetting.SupportedPrefix;

                    foreach (var prefixProvider in prefixProviders)
                    {
                        await InsertAffInfosAsync(validAffs, prefixProvider, sitePrefixProvider);
                    }
                }

                if (importData.Count() == PAGE_SIZE_DEFAULT)
                {
                    pageIndex++;
                    importData = await GetDataAsync(sitePrefixProvider, pageIndex);
                }
                else
                {
                    importData = null;
                }
            }
        }

        private async Task<IEnumerable<AffInfo>> GetDataAsync(string sitePrefixProvider, int pageIndex = PAGE_INDEX_DEFAULT)
        {
            return await _affInfoRepository.GetAffInfosAsync(new GetAffInfosCriteria(sitePrefixProvider)
            {
                PageIndex = pageIndex,
                PageSize = PAGE_SIZE_DEFAULT,
            });
        }

        private async Task InsertAffInfosAsync(IEnumerable<AffInfo> validAffInfos, string prefixProvider, string sitePrefixProvider)
        {
            var affInfosJson = JsonConvert.SerializeObject(validAffInfos);
            var command = new InsertDataTrackerCommand(prefixProvider)
            {
                PushDataInfo = affInfosJson,
                PushDataType = DataTrackerType.InsertAffInfos,
            };

            await _trackerService.InsertDataTrackerAsync(command);

            var item = new DataTrackerItem
            {
                PrefixSiteName = prefixProvider,
                PushTrackerId = command.PushTrackerId,
            };

            try
            {
                if (command.ErrCode == 0)
                {
                    var result = await _liceseeApi.InsertAffInfos(validAffInfos);

                    if (result.IsSuccess)
                    {
                        await _trackerService.UpdateTrackerStatusAsync(item, DataTrackerStatus.Success);
                    }
                    else
                    {
                        _logger.LogError($"Insert aff infos to {sitePrefixProvider} DB fail - {result.ErrMsg}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Insert aff infos to {sitePrefixProvider} DB throw exception - {ex}");

                await _trackerService.UpdateTrackerStatusAsync(item, DataTrackerStatus.Failure);
            }
        }
    }
}
