using Aff12B.CronJobs.Core.Domain.DTOs;

namespace Aff12B.Common.Tests.Builders
{
    /// <summary>
    /// Builder pattern for creating AffInfo instances in tests
    /// </summary>
    public class AffInfoBuilder
    {
        private AffInfo _affInfo;

        public AffInfoBuilder()
        {
            _affInfo = new AffInfo
            {
                AffId = 1,
                AffCode = "AFF001",
                ParentAff = "PARENT001",
                UserName = "testaffiliate",
                Player_CustId = 12345,
                Player_Username = "testplayer",
                FirstName = "Test",
                LastName = "Affiliate",
                Phone = "+1234567890",
                Email = "test@affiliate.com",
                Status = "Active"
            };
        }

        public AffInfoBuilder WithAffId(int affId)
        {
            _affInfo.AffId = affId;
            return this;
        }

        public AffInfoBuilder WithAffCode(string affCode)
        {
            _affInfo.AffCode = affCode;
            return this;
        }

        public AffInfoBuilder WithParentAff(string parentAff)
        {
            _affInfo.ParentAff = parentAff;
            return this;
        }

        public AffInfoBuilder WithUserName(string userName)
        {
            _affInfo.UserName = userName;
            return this;
        }

        public AffInfoBuilder WithPlayer(int custId, string username)
        {
            _affInfo.Player_CustId = custId;
            _affInfo.Player_Username = username;
            return this;
        }

        public AffInfoBuilder WithName(string firstName, string lastName)
        {
            _affInfo.FirstName = firstName;
            _affInfo.LastName = lastName;
            return this;
        }

        public AffInfoBuilder WithPhone(string phone)
        {
            _affInfo.Phone = phone;
            return this;
        }

        public AffInfoBuilder WithEmail(string email)
        {
            _affInfo.Email = email;
            return this;
        }

        public AffInfoBuilder WithStatus(string status)
        {
            _affInfo.Status = status;
            return this;
        }

        public AffInfoBuilder AsActive()
        {
            _affInfo.Status = "Active";
            return this;
        }

        public AffInfoBuilder AsInactive()
        {
            _affInfo.Status = "Inactive";
            return this;
        }

        public AffInfoBuilder AsPending()
        {
            _affInfo.Status = "Pending";
            return this;
        }

        public AffInfoBuilder WithoutParent()
        {
            _affInfo.ParentAff = null;
            return this;
        }

        public AffInfo Build()
        {
            return _affInfo;
        }
    }
}
