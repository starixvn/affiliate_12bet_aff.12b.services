using Aff12B.Common.DataDomain.Params;
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
        private const int PAGE_SIZE_DEFAULT = 100;
        private readonly Dictionary<string, int> SITE_IDs = new Dictionary<string, int>
        {
            { "12B", 101 }
        };

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

            _logger.LogWarning($"Start importing Affiliate Info to 12B - {DateTimeOffset.Now.ToString(Format.DateTimeFormat)}");

            foreach (var site in SupportedSites)
            {
                var sitePrefixProvider = _licenseeSiteSetting.GetCodePrefix(site);

                var affInfos = await GetDataAsync(sitePrefixProvider);
                await ImportDataByBatchAsync(affInfos, sitePrefixProvider);
            }

            _logger.LogWarning($"Complete importing Affiliate Info to 12B - {DateTimeOffset.Now.ToString(Format.DateTimeFormat)}");
        }

        private async Task ImportDataByBatchAsync(IEnumerable<AffInfo> data, string sitePrefixProvider)
        {
            var pageIndex = PAGE_INDEX_DEFAULT;
            var importData = data;

            while (importData != null && importData.Any())
            {
                var prefixProviders = FetchServiceSetting.SupportedPrefix;

                foreach (var prefixProvider in prefixProviders)
                {
                    await ImportDataToMultipleRepoAsync(importData, prefixProvider, sitePrefixProvider);
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
            return await _affInfoRepository.GetAffInfoListAsync(new AffInfoCriteria(sitePrefixProvider)
            {
                PageIndex = pageIndex,
                PageSize = PAGE_SIZE_DEFAULT,
            });
        }

        protected async Task ImportDataToMultipleRepoAsync(IEnumerable<AffInfo> data, string prefixProvider, string sitePrefixProvider)
        {
            if (!data.Any())
            {
                return;
            }

            var importTasks = data.Where(d => d.Player_CustId != 0).Select(g => InsertAffiliateInfoAsync(g, prefixProvider, sitePrefixProvider));

            await Task.WhenAll(importTasks);
        }

        private async Task InsertAffiliateInfoAsync(AffInfo affInfo, string prefixProvider, string sitePrefixProvider)
        {
            var affiliateParam = new AffiliateParam
            {
                CustId = affInfo.Player_CustId,
                SiteId = SITE_IDs[sitePrefixProvider],
                AffiliateId = affInfo.AffId,
                AffiliateName = affInfo.UserName,
                ParentAffiliate = affInfo.ParentAff,
                AffiliateCode = affInfo.AffCode,
                UserName = affInfo.Player_Username,
                FirstName = affInfo.FirstName,
                LastName = affInfo.LastName,
                ContactNumber = affInfo.Phone,
                Email = affInfo.Email,
                Status = affInfo.Status,
            };

            var command = new InsertDataTrackerCommand(prefixProvider)
            {
                PushDataInfo = JsonConvert.SerializeObject(affiliateParam),
                PushDataType = DataTrackerType.InsertAffiliate,
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
                    var result = await _liceseeApi.InsertAffiliateInfo(affiliateParam);

                    if (result.IsSuccess)
                    {
                        await _trackerService.UpdateTrackerStatusAsync(item, DataTrackerStatus.Success);
                    }
                    else
                    {
                        _logger.LogError($"Insert affiliate to {sitePrefixProvider} DB fail - {result.ErrMsg}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Insert affiliate to {sitePrefixProvider} DB throw exception - {ex}");

                await _trackerService.UpdateTrackerStatusAsync(item, DataTrackerStatus.Failure);
            }
        }
    }
}
