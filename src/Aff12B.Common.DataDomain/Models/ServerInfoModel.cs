namespace Aff12B.Common.DataDomain.Models
{
    public class ServerInfoModel
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string LangList { get; set; }

        public string CurrencyList { get; set; }

        public string DefLang { get; set; }

        public int? DefCurrency { get; set; }
    }
}
