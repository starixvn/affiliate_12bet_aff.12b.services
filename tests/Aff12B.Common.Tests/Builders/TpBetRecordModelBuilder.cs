using Aff12B.Common.DataDomain.Models;

namespace Aff12B.Common.Tests.Builders
{
    /// <summary>
    /// Builder pattern for creating TpBetRecordModel instances in tests
    /// </summary>
    public class TpBetRecordModelBuilder
    {
        private TpBetRecordModel _betRecord;

        public TpBetRecordModelBuilder()
        {
            _betRecord = new TpBetRecordModel
            {
                TpId = 1,
                TransId = "BET_001",
                GameType = 1,
                CustId = "12345",
                UserName = "testuser",
                CurrencyId = 1,
                SiteId = 1,
                TpUserName = "tp_testuser",
                TransTime = DateTime.UtcNow.AddHours(-2),
                SettledTime = DateTime.UtcNow.AddHours(-1),
                Stake = 100.00m,
                WinlostAmount = 50.00m,
                SettlementType = 1,
                TicketStatus = 1,
                OriginalState = "SETTLED",
                TicketKey = 123456,
                CreateTime = DateTime.UtcNow.AddHours(-2),
                ValidBet = 100.00m,
                BonusId = null,
                Revenue = 50.00m,
                ProgressiveBet = 0.00m,
                ProgressiveWin = 0.00m
            };
        }

        public TpBetRecordModelBuilder WithTpId(int tpId)
        {
            _betRecord.TpId = tpId;
            return this;
        }

        public TpBetRecordModelBuilder WithTransId(string transId)
        {
            _betRecord.TransId = transId;
            return this;
        }

        public TpBetRecordModelBuilder WithGameType(int gameType)
        {
            _betRecord.GameType = gameType;
            return this;
        }

        public TpBetRecordModelBuilder WithCustomer(string custId, string userName)
        {
            _betRecord.CustId = custId;
            _betRecord.UserName = userName;
            return this;
        }

        public TpBetRecordModelBuilder WithTpUserName(string tpUserName)
        {
            _betRecord.TpUserName = tpUserName;
            return this;
        }

        public TpBetRecordModelBuilder WithCurrency(int? currencyId)
        {
            _betRecord.CurrencyId = currencyId;
            return this;
        }

        public TpBetRecordModelBuilder WithSite(int? siteId)
        {
            _betRecord.SiteId = siteId;
            return this;
        }

        public TpBetRecordModelBuilder WithTransTime(DateTime? transTime)
        {
            _betRecord.TransTime = transTime;
            return this;
        }

        public TpBetRecordModelBuilder WithSettledTime(DateTime? settledTime)
        {
            _betRecord.SettledTime = settledTime;
            return this;
        }

        public TpBetRecordModelBuilder WithAmounts(decimal? stake, decimal? winlostAmount)
        {
            _betRecord.Stake = stake;
            _betRecord.WinlostAmount = winlostAmount;
            return this;
        }

        public TpBetRecordModelBuilder WithValidBet(decimal? validBet)
        {
            _betRecord.ValidBet = validBet;
            return this;
        }

        public TpBetRecordModelBuilder WithRevenue(decimal? revenue)
        {
            _betRecord.Revenue = revenue;
            return this;
        }

        public TpBetRecordModelBuilder WithSettlementType(int? settlementType)
        {
            _betRecord.SettlementType = settlementType;
            return this;
        }

        public TpBetRecordModelBuilder WithTicketStatus(int? ticketStatus)
        {
            _betRecord.TicketStatus = ticketStatus;
            return this;
        }

        public TpBetRecordModelBuilder WithOriginalState(string originalState)
        {
            _betRecord.OriginalState = originalState;
            return this;
        }

        public TpBetRecordModelBuilder WithTicketKey(int? ticketKey)
        {
            _betRecord.TicketKey = ticketKey;
            return this;
        }

        public TpBetRecordModelBuilder WithBonusId(int? bonusId)
        {
            _betRecord.BonusId = bonusId;
            return this;
        }

        public TpBetRecordModelBuilder WithProgressiveAmounts(decimal? progressiveBet, decimal? progressiveWin)
        {
            _betRecord.ProgressiveBet = progressiveBet;
            _betRecord.ProgressiveWin = progressiveWin;
            return this;
        }

        public TpBetRecordModelBuilder TransactionHoursAgo(int hours)
        {
            _betRecord.TransTime = DateTime.UtcNow.AddHours(-hours);
            _betRecord.CreateTime = _betRecord.TransTime;
            return this;
        }

        public TpBetRecordModelBuilder SettledHoursAgo(int hours)
        {
            _betRecord.SettledTime = DateTime.UtcNow.AddHours(-hours);
            return this;
        }

        public TpBetRecordModelBuilder AsWinningBet(decimal winAmount)
        {
            _betRecord.WinlostAmount = winAmount;
            _betRecord.TicketStatus = 1; // Won
            _betRecord.OriginalState = "WON";
            return this;
        }

        public TpBetRecordModelBuilder AsLosingBet()
        {
            _betRecord.WinlostAmount = -(_betRecord.Stake ?? 0);
            _betRecord.TicketStatus = 2; // Lost
            _betRecord.OriginalState = "LOST";
            return this;
        }

        public TpBetRecordModelBuilder AsPendingBet()
        {
            _betRecord.SettledTime = null;
            _betRecord.WinlostAmount = null;
            _betRecord.TicketStatus = 0; // Pending
            _betRecord.OriginalState = "PENDING";
            return this;
        }

        public TpBetRecordModelBuilder WithBonusBet(int bonusId)
        {
            _betRecord.BonusId = bonusId;
            return this;
        }

        public TpBetRecordModel Build()
        {
            return _betRecord;
        }
    }
}
