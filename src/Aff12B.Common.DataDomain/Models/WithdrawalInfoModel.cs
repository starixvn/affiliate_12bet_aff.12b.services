namespace Aff12B.Common.DataDomain.Models
{
    public class WithdrawalInfoModel : IHasUpdateTime
    {
        public int WithdrawalId { get; set; }

        public string TransId { get; set; }

        public int? WithdrawalType { get; set; }

        public int? CustId { get; set; }

        public string UserName { get; set; }

        public int? CurrencyId { get; set; }

        public int? SiteID { get; set; }

        public string WithdrawalInfo { get; set; }

        public decimal? Amount { get; set; }

        public string RefId { get; set; }

        public decimal? ChangeFee { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string Memo { get; set; }

        public string Comment { get; set; }

        public int? Status { get; set; }
    }
}
