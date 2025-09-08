using Aff12B.Common.DataDomain.Models;

namespace Aff12B.Common.Tests.Builders
{
    /// <summary>
    /// Builder pattern for creating BonusModel instances in tests
    /// </summary>
    public class BonusModelBuilder
    {
        private BonusModel _bonusModel;

        public BonusModelBuilder()
        {
            _bonusModel = new BonusModel
            {
                BonusId = 1,
                TransId = "TXN_001",
                PromotionType = 1,
                BonusCode = "WELCOME100",
                CustId = 12345,
                UserName = "testuser",
                CurrencyId = 1,
                SiteId = 1,
                LockedAmount = 100.00m,
                BonusAmount = 50.00m,
                ExpiryTime = DateTime.UtcNow.AddDays(30),
                BonusInfo = "Welcome bonus",
                Memo = "Test bonus",
                CreateTime = DateTime.UtcNow,
                Status = 1
            };
        }

        public BonusModelBuilder WithBonusId(int bonusId)
        {
            _bonusModel.BonusId = bonusId;
            return this;
        }

        public BonusModelBuilder WithTransId(string transId)
        {
            _bonusModel.TransId = transId;
            return this;
        }

        public BonusModelBuilder WithPromotionType(int promotionType)
        {
            _bonusModel.PromotionType = promotionType;
            return this;
        }

        public BonusModelBuilder WithBonusCode(string bonusCode)
        {
            _bonusModel.BonusCode = bonusCode;
            return this;
        }

        public BonusModelBuilder WithCustomer(int custId, string userName)
        {
            _bonusModel.CustId = custId;
            _bonusModel.UserName = userName;
            return this;
        }

        public BonusModelBuilder WithCurrency(int currencyId)
        {
            _bonusModel.CurrencyId = currencyId;
            return this;
        }

        public BonusModelBuilder WithSite(int siteId)
        {
            _bonusModel.SiteId = siteId;
            return this;
        }

        public BonusModelBuilder WithAmounts(decimal lockedAmount, decimal bonusAmount)
        {
            _bonusModel.LockedAmount = lockedAmount;
            _bonusModel.BonusAmount = bonusAmount;
            return this;
        }

        public BonusModelBuilder WithExpiryTime(DateTime expiryTime)
        {
            _bonusModel.ExpiryTime = expiryTime;
            return this;
        }

        public BonusModelBuilder WithBonusInfo(string bonusInfo)
        {
            _bonusModel.BonusInfo = bonusInfo;
            return this;
        }

        public BonusModelBuilder WithMemo(string memo)
        {
            _bonusModel.Memo = memo;
            return this;
        }

        public BonusModelBuilder WithCreateTime(DateTime createTime)
        {
            _bonusModel.CreateTime = createTime;
            return this;
        }

        public BonusModelBuilder WithStatus(int status)
        {
            _bonusModel.Status = status;
            return this;
        }

        public BonusModelBuilder ExpiresInDays(int days)
        {
            _bonusModel.ExpiryTime = DateTime.UtcNow.AddDays(days);
            return this;
        }

        public BonusModelBuilder CreatedDaysAgo(int days)
        {
            _bonusModel.CreateTime = DateTime.UtcNow.AddDays(-days);
            return this;
        }

        public BonusModelBuilder AsActive()
        {
            _bonusModel.Status = 1;
            return this;
        }

        public BonusModelBuilder AsExpired()
        {
            _bonusModel.Status = 0;
            _bonusModel.ExpiryTime = DateTime.UtcNow.AddDays(-1);
            return this;
        }

        public BonusModel Build()
        {
            return _bonusModel;
        }
    }
}
