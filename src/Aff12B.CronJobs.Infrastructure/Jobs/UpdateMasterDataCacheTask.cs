using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Fanex.Caching;
using Fanex.Data.Repository;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public class UpdateMasterDataCacheTask : ICacheUpdateTask
    {
        private const string MASTER_DATA_CACHE_KEY_TEMPLATE = "Aff.{0}.Go.MasterData";
        private const int CacheDurationInSeconds = 86400;

        private readonly ICacheService _cacheService;
        private readonly IDynamicRepository _dynamicRepository;
        private readonly ILogger<UpdateMasterDataCacheTask> _logger;

        private readonly LicenseeSiteSetting _licenseeSiteSetting;

        private IEnumerable<string> SupportedSites => _licenseeSiteSetting?.SupportedSites;

        public UpdateMasterDataCacheTask(
            ICacheService cacheService,
            IDynamicRepository dynamicRepository,
            IOptions<LicenseeSiteSetting> config,
            ILogger<UpdateMasterDataCacheTask> logger)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _dynamicRepository = dynamicRepository ?? throw new ArgumentNullException(nameof(dynamicRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _licenseeSiteSetting = config.Value ?? throw new NullReferenceException(nameof(_licenseeSiteSetting));
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
                foreach (var site in SupportedSites)
                {
                    var sitePrefixProvider = _licenseeSiteSetting.GetCodePrefix(site);

                    // Fetch master data from stored procedure for this site
                    var masterData = await GetMasterDataAsync(sitePrefixProvider);

                    if (masterData != null)
                    {
                        // Update cache with site-specific key
                        var siteSpecificCacheKey = string.Format(MASTER_DATA_CACHE_KEY_TEMPLATE, sitePrefixProvider);
                        await UpdateCacheForSiteAsync(siteSpecificCacheKey, masterData, sitePrefixProvider);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update master data cache");
                throw;
            }
        }

        private async Task UpdateCacheForSiteAsync(string cacheKey, IList<MasterInfoValidation> masterData, string sitePrefixProvider)
        {
            try
            {
                var cacheOptions = new CacheItemOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CacheDurationInSeconds)
                };
                await _cacheService.SetAsync(cacheKey, masterData, cacheOptions);

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

        private async Task<IList<MasterInfoValidation>> GetMasterDataAsync(string sitePrefixProvider)
        {
            try
            {
                // Fetch master data from stored procedure using site prefix
                var criteria = new MasterInfoValidationCriteria(sitePrefixProvider);
                var masterInfoValidationData = await _dynamicRepository.FetchAsync<MasterInfoValidation>(criteria);

                return masterInfoValidationData?.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch master data from stored procedure for site: {Site}", sitePrefixProvider);
                throw;
            }
        }
    }
}