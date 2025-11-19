using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    /// <summary>
    /// Task responsible for updating master data cache for all supported sites.
    /// Processes sites in parallel and handles errors gracefully.
    /// </summary>
    public class UpdateMasterDataCacheTask : IUpdateDataCacheTask
    {
        private const string CacheKeyTemplate = "Aff.{0}.Go.MasterData";
        private const int CacheDurationInSeconds = 86400;
        private const int MaxConcurrency = 10;

        private readonly IMasterDataProviderService _masterDataProviderService;
        private readonly IDataCacheService _dataCacheService;
        private readonly LicenseeSiteSetting _licenseeSiteSetting;
        private readonly ILogger<UpdateMasterDataCacheTask> _logger;

        private IEnumerable<string> SupportedSites => _licenseeSiteSetting?.SupportedSites ?? [];

        public UpdateMasterDataCacheTask(
            IMasterDataProviderService masterDataProviderService,
            IDataCacheService dataCacheService,
            IOptions<LicenseeSiteSetting> config,
            ILogger<UpdateMasterDataCacheTask> logger)
        {
            _masterDataProviderService = masterDataProviderService ?? throw new ArgumentNullException(nameof(masterDataProviderService));
            _dataCacheService = dataCacheService ?? throw new ArgumentNullException(nameof(dataCacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _licenseeSiteSetting = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task UpdateDataCacheAsync()
        {
            var sites = SupportedSites.ToList();

            if (!sites.Any())
            {
                _logger.LogInformation("No supported sites found, skipping cache update.");
                return;
            }

            _logger.LogInformation("Starting master data cache update for {Count} sites: {Sites}",
                sites.Count, string.Join(", ", sites));

            var errors = new ConcurrentBag<string>();
            var successCount = 0;
            var options = new ParallelOptions { MaxDegreeOfParallelism = MaxConcurrency };

            await Parallel.ForEachAsync(sites, options, async (site, _) =>
            {
                try
                {
                    await ProcessSiteAsync(site);
                    Interlocked.Increment(ref successCount);
                }
                catch (Exception ex)
                {
                    errors.Add(site);
                    _logger.LogError(ex, "Failed to process site: {Site}", site);
                }
            });

            LogResults(sites.Count, successCount, errors);
        }

        private async Task ProcessSiteAsync(string site)
        {
            if (string.IsNullOrWhiteSpace(site))
            {
                throw new ArgumentException("Site name cannot be null or empty", nameof(site));
            }

            var sitePrefix = _licenseeSiteSetting.GetCodePrefix(site);
            if (string.IsNullOrWhiteSpace(sitePrefix))
            {
                throw new InvalidOperationException($"Unable to determine code prefix for site: {site}");
            }

            var masterData = await _masterDataProviderService.GetMasterDataAsync(sitePrefix);
            if (masterData == null)
            {
                _logger.LogWarning("No master data found for site: {Site} (prefix: {Prefix})", site, sitePrefix);
                return;
            }

            var cacheKey = string.Format(CacheKeyTemplate, sitePrefix);
            var success = await _dataCacheService.UpdateAsync(cacheKey, masterData, TimeSpan.FromSeconds(CacheDurationInSeconds));

            if (!success)
            {
                _logger.LogWarning("Cache update failed for site: {Site} (key: {Key})", site, cacheKey);
            }
        }

        private void LogResults(int total, int successCount, ConcurrentBag<string> errors)
        {
            if (errors.IsEmpty)
            {
                _logger.LogInformation("All {Count} sites updated successfully.", total);
            }
            else
            {
                _logger.LogWarning("Update completed with errors. Success: {Success}/{Total}, Failed: {Failed}",
                    successCount, total, errors.Count);

                foreach (var site in errors)
                {
                    _logger.LogError("Failed site: {Site}", site);
                }
            }
        }
    }
}