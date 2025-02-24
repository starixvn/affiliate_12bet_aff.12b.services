using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Command
{
    public class UnlockUsersCommand : NonQueryCommand
    {
        public string CustIdList { get; set; }

        public override string GetSettingKey() => "API_Customer_Unlock";

        public override bool IsValid() => true;
    }
}
