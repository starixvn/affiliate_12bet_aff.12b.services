using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Aff12B.Common.DataDomain.Params
{
    public class AffiliateParam
    {
        private const int MARKETING_CUST_ID = -1;

        [JsonProperty("SiteId")]
        public int SiteId { get; set; }

        [JsonProperty("AffiliateId")]
        public int AffId { get; set; }

        [Required]
        [JsonProperty("AffiliateCode")]
        public string AffCode { get; set; }

        [Required]
        [JsonProperty("AffiliateName")]
        public string UserName { get; set; }

        [JsonProperty("ParentAffiliate")]
        public string ParentAff { get; set; } = string.Empty;

        [JsonIgnore]
        public int Player_CustId { get; set; }

        [JsonProperty("CustId")]
        public int CustId => Player_CustId <= 0 ? MARKETING_CUST_ID : Player_CustId;

        [JsonProperty("UserName")]
        public string Player_Username { get; set; } = string.Empty;

        [JsonProperty("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("LastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("ContactNumber")]
        public string Phone { get; set; } = string.Empty;

        [JsonProperty("Email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("Status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("DealVerifyFlags")]
        public int DealVerifyFlags { get; set; }
    }
}
