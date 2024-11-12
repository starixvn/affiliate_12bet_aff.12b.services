using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetTpGameTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "API_Get_TpGameType";

        public override bool IsValid() => true;
    }
}
