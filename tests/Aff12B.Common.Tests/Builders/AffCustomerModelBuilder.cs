using Aff12B.Common.DataDomain.Models;

namespace Aff12B.Common.Tests.Builders
{
    /// <summary>
    /// Builder pattern for creating AffCustomerModel instances in tests
    /// </summary>
    public class AffCustomerModelBuilder
    {
        private AffCustomerModel _affCustomerModel;

        public AffCustomerModelBuilder()
        {
            _affCustomerModel = new AffCustomerModel
            {
                CustId = 1,
                UserName = "testuser",
                ClickId = 12345,
                FirstDepositDate = DateTime.UtcNow.AddDays(-7),
                RegTime = DateTime.UtcNow.AddDays(-14)
            };
        }

        /// <summary>
        /// Sets the customer ID
        /// </summary>
        /// <param name="custId">Customer ID</param>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder WithCustId(int custId)
        {
            _affCustomerModel.CustId = custId;
            return this;
        }

        /// <summary>
        /// Sets the username
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder WithUserName(string userName)
        {
            _affCustomerModel.UserName = userName;
            return this;
        }

        /// <summary>
        /// Sets the click ID
        /// </summary>
        /// <param name="clickId">Click ID</param>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder WithClickId(long? clickId)
        {
            _affCustomerModel.ClickId = clickId;
            return this;
        }

        /// <summary>
        /// Sets the first deposit date
        /// </summary>
        /// <param name="firstDepositDate">First deposit date</param>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder WithFirstDepositDate(DateTime? firstDepositDate)
        {
            _affCustomerModel.FirstDepositDate = firstDepositDate;
            return this;
        }

        /// <summary>
        /// Sets the registration time
        /// </summary>
        /// <param name="regTime">Registration time</param>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder WithRegTime(DateTime? regTime)
        {
            _affCustomerModel.RegTime = regTime;
            return this;
        }

        /// <summary>
        /// Sets first deposit date to a specific number of days ago
        /// </summary>
        /// <param name="daysAgo">Number of days ago</param>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder FirstDepositDaysAgo(int daysAgo)
        {
            _affCustomerModel.FirstDepositDate = DateTime.UtcNow.AddDays(-daysAgo);
            return this;
        }

        /// <summary>
        /// Sets registration time to a specific number of days ago
        /// </summary>
        /// <param name="daysAgo">Number of days ago</param>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder RegisteredDaysAgo(int daysAgo)
        {
            _affCustomerModel.RegTime = DateTime.UtcNow.AddDays(-daysAgo);
            return this;
        }

        /// <summary>
        /// Creates a customer without first deposit
        /// </summary>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder WithoutFirstDeposit()
        {
            _affCustomerModel.FirstDepositDate = null;
            return this;
        }

        /// <summary>
        /// Creates a customer without click ID
        /// </summary>
        /// <returns>Builder instance</returns>
        public AffCustomerModelBuilder WithoutClickId()
        {
            _affCustomerModel.ClickId = null;
            return this;
        }

        /// <summary>
        /// Builds the AffCustomerModel instance
        /// </summary>
        /// <returns>Configured AffCustomerModel</returns>
        public AffCustomerModel Build()
        {
            return _affCustomerModel;
        }
    }
}
