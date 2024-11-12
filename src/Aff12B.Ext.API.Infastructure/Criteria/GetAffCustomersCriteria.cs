using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetAffCustomersCriteria : CriteriaBase
    {
        private const string SP_NAME = "Aff_CustomerSel";

        public GetAffCustomersCriteria(DateTime fromDate)
            => FromDate = fromDate;


        public override string GetSettingKey() => SP_NAME;

        public override bool IsValid() => true;

        public DateTime FromDate { get; }
    }
}