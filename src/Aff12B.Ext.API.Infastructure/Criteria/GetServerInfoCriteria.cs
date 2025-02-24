using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetServerInfoCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "API_Get_ServerInfo";

        public override bool IsValid() => true;
    }
}
