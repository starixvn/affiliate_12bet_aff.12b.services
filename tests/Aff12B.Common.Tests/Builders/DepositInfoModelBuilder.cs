using Aff12B.Common.DataDomain.Models;

namespace Aff12B.Common.Tests.Builders
{
    /// <summary>
    /// Builder pattern for creating DepositInfoModel instances in tests
    /// </summary>
    public class DepositInfoModelBuilder
    {
        private DepositInfoModel _depositInfo;

        public DepositInfoModelBuilder()
        {
            _depositInfo = new DepositInfoModel
            {
                DepositId = 1,
                TransId = "DEP_001",
                DepositType = 1,
                CustId = 12345,
                UserName = "testuser",
                CurrencyId = 1,
                SiteId = 1,
                DespositInfo = "Credit Card Deposit",
                Amount = 100.00m,
                ActualDeposit = 100.00m,
                CreateTime = DateTime.UtcNow,
                TPCreateTime = DateTime.UtcNow,
                CallbackTime = DateTime.UtcNow.AddMinutes(5),
                FirstDeposit = 1,
                ThirdId = "THIRD_001",
                RefId = "REF_001",
                DepositTime = DateTime.UtcNow,
                Memo = "Test deposit",
                PostScript = null,
                Comment = null,
                Status = 1,
                Flag = 0,
                Operator = "SYSTEM",
                UpdateTime = DateTime.UtcNow
            };
        }

        public DepositInfoModelBuilder WithDepositId(int depositId)
        {
            _depositInfo.DepositId = depositId;
            return this;
        }

        public DepositInfoModelBuilder WithTransId(string transId)
        {
            _depositInfo.TransId = transId;
            return this;
        }

        public DepositInfoModelBuilder WithDepositType(int? depositType)
        {
            _depositInfo.DepositType = depositType;
            return this;
        }

        public DepositInfoModelBuilder WithCustomer(int? custId, string userName)
        {
            _depositInfo.CustId = custId;
            _depositInfo.UserName = userName;
            return this;
        }

        public DepositInfoModelBuilder WithCurrency(int? currencyId)
        {
            _depositInfo.CurrencyId = currencyId;
            return this;
        }

        public DepositInfoModelBuilder WithSite(int? siteId)
        {
            _depositInfo.SiteId = siteId;
            return this;
        }

        public DepositInfoModelBuilder WithDepositInfo(string despositInfo)
        {
            _depositInfo.DespositInfo = despositInfo;
            return this;
        }

        public DepositInfoModelBuilder WithAmounts(decimal? amount, decimal? actualDeposit)
        {
            _depositInfo.Amount = amount;
            _depositInfo.ActualDeposit = actualDeposit;
            return this;
        }

        public DepositInfoModelBuilder WithCreateTime(DateTime? createTime)
        {
            _depositInfo.CreateTime = createTime;
            return this;
        }

        public DepositInfoModelBuilder WithTPCreateTime(DateTime? tpCreateTime)
        {
            _depositInfo.TPCreateTime = tpCreateTime;
            return this;
        }

        public DepositInfoModelBuilder WithCallbackTime(DateTime? callbackTime)
        {
            _depositInfo.CallbackTime = callbackTime;
            return this;
        }

        public DepositInfoModelBuilder WithFirstDeposit(int? firstDeposit)
        {
            _depositInfo.FirstDeposit = firstDeposit;
            return this;
        }

        public DepositInfoModelBuilder WithThirdPartyIds(string thirdId, string refId)
        {
            _depositInfo.ThirdId = thirdId;
            _depositInfo.RefId = refId;
            return this;
        }

        public DepositInfoModelBuilder WithDepositTime(DateTime? depositTime)
        {
            _depositInfo.DepositTime = depositTime;
            return this;
        }

        public DepositInfoModelBuilder WithMemo(string memo)
        {
            _depositInfo.Memo = memo;
            return this;
        }

        public DepositInfoModelBuilder WithPostScript(string postScript)
        {
            _depositInfo.PostScript = postScript;
            return this;
        }

        public DepositInfoModelBuilder WithComment(string comment)
        {
            _depositInfo.Comment = comment;
            return this;
        }

        public DepositInfoModelBuilder WithStatus(int? status)
        {
            _depositInfo.Status = status;
            return this;
        }

        public DepositInfoModelBuilder WithFlag(int? flag)
        {
            _depositInfo.Flag = flag;
            return this;
        }

        public DepositInfoModelBuilder WithOperator(string operatorName)
        {
            _depositInfo.Operator = operatorName;
            return this;
        }

        public DepositInfoModelBuilder WithUpdateTime(DateTime? updateTime)
        {
            _depositInfo.UpdateTime = updateTime;
            return this;
        }

        public DepositInfoModelBuilder CreatedHoursAgo(int hours)
        {
            var createTime = DateTime.UtcNow.AddHours(-hours);
            _depositInfo.CreateTime = createTime;
            _depositInfo.TPCreateTime = createTime;
            _depositInfo.DepositTime = createTime;
            return this;
        }

        public DepositInfoModelBuilder AsFirstDeposit()
        {
            _depositInfo.FirstDeposit = 1;
            return this;
        }

        public DepositInfoModelBuilder AsSubsequentDeposit()
        {
            _depositInfo.FirstDeposit = 0;
            return this;
        }

        public DepositInfoModelBuilder AsSuccessful()
        {
            _depositInfo.Status = 1;
            _depositInfo.CallbackTime = DateTime.UtcNow;
            return this;
        }

        public DepositInfoModelBuilder AsPending()
        {
            _depositInfo.Status = 0;
            _depositInfo.CallbackTime = null;
            return this;
        }

        public DepositInfoModelBuilder AsFailed()
        {
            _depositInfo.Status = -1;
            _depositInfo.ActualDeposit = 0;
            return this;
        }

        public DepositInfoModelBuilder WithPartialAmount(decimal requestedAmount, decimal actualAmount)
        {
            _depositInfo.Amount = requestedAmount;
            _depositInfo.ActualDeposit = actualAmount;
            return this;
        }

        public DepositInfoModel Build()
        {
            return _depositInfo;
        }
    }
}
