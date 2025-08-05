namespace Aff12B.CronJobs.Core.Domain.DTOs.AffLink
{
    public class MasterInfoValidation
    {
        public IEnumerable<SpecialDomainMapping> SpecialDomainMappings { get; set; }

        public IEnumerable<DomainMapping> DomainMappings { get; set; }

        public IEnumerable<LanguageReference> LanguageReferences { get; set; }

        public IEnumerable<AffiliateCommGroup> AffCommGroups { get; set; }
    }
}