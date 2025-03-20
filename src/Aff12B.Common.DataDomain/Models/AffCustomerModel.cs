namespace Aff12B.Common.DataDomain.Models
{
    public class AffCustomerModel
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public int CustId { get; set; }

        public string UserName { get; set; }

        public int? CurrencyId { get; set; }

        public int? ActStatus { get; set; }

        public DateTime? RegTime { get; set; }

        public DateTime? FirstDepositDate { get; set; }

        public int? Status { get; set; }

        public int? AffId { get; set; }

        public bool? IsTest { get; set; }

        public string AffCode { get; set; }

        public int? AffMediaLinkId { get; set; }
    }
}