using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Aff12B.CronJobs.Infrastructure.Services;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Fanex.Data.Repository;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public class UpdateMasterDataCacheTask : ICacheUpdateTask
    {
        private const string MASTER_DATA_CACHE_KEY_TEMPLATE = "Aff.{0}.Go.MasterData";
        private const int CacheSlidingExpirationInHours = 24;

        private readonly IRedisService _redisService;
        private readonly IDynamicRepository _dynamicRepository;
        private readonly ILogger<UpdateMasterDataCacheTask> _logger;

        private readonly LicenseeSiteSetting _licenseeSiteSetting;

        private IEnumerable<string> SupportedSites => _licenseeSiteSetting?.SupportedSites;

        public UpdateMasterDataCacheTask(
            IRedisService redisService,
            IDynamicRepository dynamicRepository,
            IOptions<LicenseeSiteSetting> config,
            ILogger<UpdateMasterDataCacheTask> logger)
        {
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _dynamicRepository = dynamicRepository ?? throw new ArgumentNullException(nameof(dynamicRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _licenseeSiteSetting = config.Value;
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task UpdateMasterDataCacheAsync()
        {
            if (SupportedSites == null || !SupportedSites.Any())
            {
                return;
            }

            try
            {
                await Parallel.ForEachAsync(SupportedSites, async (site, _) =>
                {
                    var sitePrefixProvider = _licenseeSiteSetting.GetCodePrefix(site);
                    var masterData = await GetMasterDataAsync(sitePrefixProvider);

                    if (masterData != null)
                    {
                        var siteSpecificCacheKey = string.Format(MASTER_DATA_CACHE_KEY_TEMPLATE, sitePrefixProvider);
                        await UpdateCacheForSiteAsync(siteSpecificCacheKey, masterData, sitePrefixProvider);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update master data cache");
                throw;
            }
        }

        private async Task UpdateCacheForSiteAsync(string cacheKey, MasterInfoValidation masterData, string sitePrefixProvider)
        {
            try
            {
                var slidingExpiration = TimeSpan.FromHours(CacheSlidingExpirationInHours);
                await _redisService.SetAsync(cacheKey, masterData, slidingExpiration);

                _logger.LogInformation("Successfully updated master data cache for key: {CacheKey}, Site: {Site}",
                    cacheKey, sitePrefixProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cache for key: {CacheKey}, Site: {Site}",
                    cacheKey, sitePrefixProvider);
                throw;
            }
        }

        private async Task<MasterInfoValidation> GetMasterDataAsync(string sitePrefixProvider)
        {
            try
            {
                var criteria = new MasterInfoValidationCriteria(sitePrefixProvider);
                var results = await _dynamicRepository.FetchMultipleAsync<
                    SpecialDomainMapping,
                    DomainMapping,
                    LanguageReference,
                    AffiliateDeal,
                    LandingPageSetting>(criteria);

                return new MasterInfoValidation
                {
                    SpecialDomainMappings = results.FirstResultSet,
                    DomainMappings = results.SecondResultSet,
                    LanguageReferences = results.ThirdResultSet,
                    AffCommGroups = results.FourthResultSet,
                    LandingPages = results.FifthResultSet
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                                "Failed to fetch master data from stored procedure for site: {Site}",
                                sitePrefixProvider);
                throw;
            }
        }
    }
}