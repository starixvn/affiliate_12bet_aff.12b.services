using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetLoginCustInfoCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "Affiliate_LoginCustInfo_Get";

        public override bool IsValid()
            => true;

        public string UserName { get; set; }

        public int SiteId { get; set; }
    }
}
