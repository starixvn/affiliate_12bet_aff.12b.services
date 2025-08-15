using Aff12B.Common.DataDomain.Models;

namespace Aff12B.Common.Tests.Builders
{
    /// <summary>
    /// Builder pattern for creating CustomerModel instances in tests
    /// </summary>
    public class CustomerModelBuilder
    {
        private CustomerModel _customerModel;

        public CustomerModelBuilder()
        {
            _customerModel = new CustomerModel
            {
                CustId = 1,
                UserName = "testuser",
                RegTime = DateTime.UtcNow.AddDays(-1),
                IsTest = false,
                TagList = null
            };
        }

        /// <summary>
        /// Sets the customer ID
        /// </summary>
        /// <param name="custId">Customer ID</param>
        /// <returns>Builder instance</returns>
        public CustomerModelBuilder WithCustId(int custId)
        {
            _customerModel.CustId = custId;
            return this;
        }

        /// <summary>
        /// Sets the username
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Builder instance</returns>
        public CustomerModelBuilder WithUserName(string userName)
        {
            _customerModel.UserName = userName;
            return this;
        }

        /// <summary>
        /// Sets the registration time
        /// </summary>
        /// <param name="regTime">Registration time</param>
        /// <returns>Builder instance</returns>
        public CustomerModelBuilder WithRegTime(DateTime? regTime)
        {
            _customerModel.RegTime = regTime;
            return this;
        }

        /// <summary>
        /// Sets whether this is a test customer
        /// </summary>
        /// <param name="isTest">Is test customer flag</param>
        /// <returns>Builder instance</returns>
        public CustomerModelBuilder WithIsTest(bool? isTest)
        {
            _customerModel.IsTest = isTest;
            return this;
        }

        /// <summary>
        /// Sets the tag list
        /// </summary>
        /// <param name="tagList">Comma-separated tag list</param>
        /// <returns>Builder instance</returns>
        public CustomerModelBuilder WithTagList(string tagList)
        {
            _customerModel.TagList = tagList;
            return this;
        }

        /// <summary>
        /// Creates a test customer (IsTest = true)
        /// </summary>
        /// <returns>Builder instance</returns>
        public CustomerModelBuilder AsTestCustomer()
        {
            _customerModel.IsTest = true;
            return this;
        }

        /// <summary>
        /// Creates a real customer (IsTest = false)
        /// </summary>
        /// <returns>Builder instance</returns>
        public CustomerModelBuilder AsRealCustomer()
        {
            _customerModel.IsTest = false;
            return this;
        }

        /// <summary>
        /// Sets registration time to a specific number of days ago
        /// </summary>
        /// <param name="daysAgo">Number of days ago</param>
        /// <returns>Builder instance</returns>
        public CustomerModelBuilder RegisteredDaysAgo(int daysAgo)
        {
            _customerModel.RegTime = DateTime.UtcNow.AddDays(-daysAgo);
            return this;
        }

        /// <summary>
        /// Builds the CustomerModel instance
        /// </summary>
        /// <returns>Configured CustomerModel</returns>
        public CustomerModel Build()
        {
            return _customerModel;
        }
    }
}
