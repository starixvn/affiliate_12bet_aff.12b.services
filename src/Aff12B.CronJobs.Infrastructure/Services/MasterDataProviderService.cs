using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Fanex.Data.Repository;
using Microsoft.Extensions.Logging;

namespace Aff12B.CronJobs.Infrastructure.Services
{
    public class MasterDataProviderService : IMasterDataProviderService
    {
        private readonly IDynamicRepository _repository;
        private readonly ILogger<MasterDataProviderService> _logger;

        public MasterDataProviderService(IDynamicRepository repository, ILogger<MasterDataProviderService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MasterInfoValidation> GetMasterDataAsync(string sitePrefix)
        {
            if (string.IsNullOrWhiteSpace(sitePrefix))
            {
                throw new ArgumentException("Site prefix cannot be null or empty", nameof(sitePrefix));
            }

            try
            {
                var criteria = new MasterInfoValidationCriteria(sitePrefix);
                var results = await _repository.FetchMultipleAsync<
                    SpecialDomainMapping,
                    DomainMapping,
                    LanguageReference,
                    AffiliateDeal,
                    LandingPageSetting>(criteria);

                return new MasterInfoValidation
                {
                    SpecialDomainMappings = results.FirstResultSet ?? [],
                    DomainMappings = results.SecondResultSet ?? [],
                    LanguageReferences = results.ThirdResultSet ?? [],
                    AffCommGroups = results.FourthResultSet ?? [],
                    LandingPages = results.FifthResultSet ?? []
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch master data for sitePrefix: {SitePrefix}", sitePrefix);
                return null;
            }
        }
    }
}
