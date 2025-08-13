namespace Aff12B.CronJobs.Core.Domain.DTOs.AffLink
{
    public class DomainMapping
    {
        public string AffDomain { get; set; }

        public string LanguageCode { get; set; }

        public string LanguageLink { get; set; }

        public string RedirectDomain { get; set; }

        public bool IsSSL { get; set; }
    }
}
