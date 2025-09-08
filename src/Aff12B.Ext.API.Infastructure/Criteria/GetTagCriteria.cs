using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetTagCriteria : CriteriaBase
    {
        public DateTime UpdatedTime { get; }

        public int BatchCount { get; }

        public GetTagCriteria(DateTime updatedTime, int batchCount)
        {
            UpdatedTime = updatedTime;
            BatchCount = batchCount;
        }

        public override string GetSettingKey() => "API_Tag_Get";

        public override bool IsValid() => true;
    }
}
