namespace Aff12B.Common.DataDomain.Models
{
    public class MasterInfoValidation
    {
        public string AffCode { get; set; }

        public string AffDomain { get; set; }

        public string LanguageCode { get; set; }

        public string LanguageLink { get; set; }

        public string RedirectDomain { get; set; }

        public bool IsSpecific { get; set; }

        public bool IsSSL { get; set; }
    }
}