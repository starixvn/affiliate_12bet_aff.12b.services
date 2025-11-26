using Fanex.Data;
using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Command
{
    public class InsertAffInfosCommand : NonQueryCommand
    {
        [SpParam(SqlParamName = "_JsonInput")]
        public string AffInfosJson { get; set; }

        public override string GetSettingKey() => "Affiliate_AffiliateInfo_Set";

        public override bool IsValid() => true;
    }
}

