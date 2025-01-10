using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Command
{
    public class InsertAffiliateInfoCommand : NonQueryCommand
    {
        public int CustId { get; set; }

        public int SiteId { get; set; }

        public int AffiliateId { get; set; }

        public string AffiliateName { get; set; }

        public string ParentAffiliate { get; set; }

        public string AffiliateCode { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ContactNumber { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public override string GetSettingKey() => "Affiliate_AffiliateInfo_Set";

        public override bool IsValid() => true;
    }
}

