using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetAffCustomersCriteria : GetCustomersCriteria
    {
        private const string SP_NAME = "Aff_CustomerSel";

        public GetAffCustomersCriteria(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
            :base(fromDate, toDate, pageIndex, pageSize)
        {
        }

        public override string GetSettingKey() => SP_NAME;
    }
}