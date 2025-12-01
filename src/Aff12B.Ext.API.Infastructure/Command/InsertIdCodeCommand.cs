namespace Aff12B.Ext.API.Infastructure.Command
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertIdCodeCommand : NonQueryCommand
    {
        public override string GetSettingKey() => "OH_Link_IDCode_Set";

        public override bool IsValid() => true;

        [SpParam(SqlParamName = "_IDCode")]
        public string IdCode { get; set; }

        [SpParam(SqlParamName = "_SiteId")]
        public int SiteId { get; set; }

        [SpParam(SqlParamName = "_StatusId")]
        public int StatusId { get; set; }
    }
}
