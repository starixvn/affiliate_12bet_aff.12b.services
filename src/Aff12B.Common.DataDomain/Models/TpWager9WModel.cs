namespace Aff12B.Common.DataDomain.Models
{
    public class TpWager9WModel
    {
        public string BetId { get; set; }

        public string UserId { get; set; }

        public string AgentId { get; set; }

        public string EventId { get; set; }

        public string MarketId { get; set; }

        public string MatchSettledDate { get; set; }

        public string BetStatus { get; set; }

        public decimal MatchStake { get; set; }

        public decimal ProfitLoss { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
