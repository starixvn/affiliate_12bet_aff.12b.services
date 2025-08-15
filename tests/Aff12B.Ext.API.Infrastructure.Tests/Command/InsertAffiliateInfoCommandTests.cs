using Aff12B.Ext.API.Infastructure.Command;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Command
{
    public class InsertAffiliateInfoCommandTests
    {
        #region Property Tests

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.CustId = 12345;
            command.SiteId = 1;
            command.AffiliateId = 67890;
            command.AffiliateName = "Test Affiliate";
            command.ParentAffiliate = "Parent Affiliate";
            command.AffiliateCode = "AFF001";
            command.UserName = "testuser";
            command.FirstName = "John";
            command.LastName = "Doe";
            command.ContactNumber = "+1234567890";
            command.Email = "john.doe@example.com";
            command.Status = "Active";

            // Assert
            command.CustId.Should().Be(12345);
            command.SiteId.Should().Be(1);
            command.AffiliateId.Should().Be(67890);
            command.AffiliateName.Should().Be("Test Affiliate");
            command.ParentAffiliate.Should().Be("Parent Affiliate");
            command.AffiliateCode.Should().Be("AFF001");
            command.UserName.Should().Be("testuser");
            command.FirstName.Should().Be("John");
            command.LastName.Should().Be("Doe");
            command.ContactNumber.Should().Be("+1234567890");
            command.Email.Should().Be("john.doe@example.com");
            command.Status.Should().Be("Active");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(12345)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void CustId_ShouldHandleVariousValues(int custId)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.CustId = custId;

            // Assert
            command.CustId.Should().Be(custId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void SiteId_ShouldHandleVariousValues(int siteId)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.SiteId = siteId;

            // Assert
            command.SiteId.Should().Be(siteId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(67890)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void AffiliateId_ShouldHandleVariousValues(int affiliateId)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.AffiliateId = affiliateId;

            // Assert
            command.AffiliateId.Should().Be(affiliateId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Test Affiliate")]
        [InlineData("Affiliate with Special Characters !@#$%")]
        [InlineData("Very Long Affiliate Name That Might Exceed Normal Limits")]
        [InlineData("中文联盟")]
        [InlineData("Affiliate123")]
        public void AffiliateName_ShouldHandleVariousValues(string affiliateName)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.AffiliateName = affiliateName;

            // Assert
            command.AffiliateName.Should().Be(affiliateName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("AFF001")]
        [InlineData("AFFILIATE_CODE_123")]
        [InlineData("aff-code-with-dashes")]
        [InlineData("AffCode!@#")]
        [InlineData("123456789")]
        public void AffiliateCode_ShouldHandleVariousValues(string affiliateCode)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.AffiliateCode = affiliateCode;

            // Assert
            command.AffiliateCode.Should().Be(affiliateCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("testuser")]
        [InlineData("user@domain.com")]
        [InlineData("user_with_underscore")]
        [InlineData("user-with-dash")]
        [InlineData("UserWithMixedCase")]
        [InlineData("用户名")]
        [InlineData("123456789")]
        public void UserName_ShouldHandleVariousValues(string userName)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.UserName = userName;

            // Assert
            command.UserName.Should().Be(userName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("john.doe@example.com")]
        [InlineData("user+tag@domain.co.uk")]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@domain.com")]
        [InlineData("very.long.email.address.that.might.exceed.normal.limits@very.long.domain.name.com")]
        public void Email_ShouldHandleVariousValues(string email)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.Email = email;

            // Assert
            command.Email.Should().Be(email);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("+1234567890")]
        [InlineData("123-456-7890")]
        [InlineData("(123) 456-7890")]
        [InlineData("123.456.7890")]
        [InlineData("1234567890")]
        [InlineData("+86 138 0013 8000")]
        [InlineData("invalid-phone")]
        public void ContactNumber_ShouldHandleVariousValues(string contactNumber)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.ContactNumber = contactNumber;

            // Assert
            command.ContactNumber.Should().Be(contactNumber);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Active")]
        [InlineData("Inactive")]
        [InlineData("Pending")]
        [InlineData("Suspended")]
        [InlineData("Deleted")]
        [InlineData("ACTIVE")]
        [InlineData("active")]
        [InlineData("1")]
        [InlineData("0")]
        public void Status_ShouldHandleVariousValues(string status)
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            command.Status = status;

            // Assert
            command.Status.Should().Be(status);
        }

        #endregion

        #region Method Tests

        [Fact]
        public void GetSettingKey_ShouldReturnCorrectValue()
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            var settingKey = command.GetSettingKey();

            // Assert
            settingKey.Should().Be("Affiliate_AffiliateInfo_Set");
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithVariousPropertyValues_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand
            {
                CustId = -1,
                SiteId = 0,
                AffiliateId = int.MaxValue,
                AffiliateName = null,
                AffiliateCode = "",
                UserName = "test",
                Email = "invalid-email",
                Status = null
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void InsertAffiliateInfoCommand_ShouldInheritFromNonQueryCommand()
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act & Assert
            command.Should().BeAssignableTo<Fanex.Data.Repository.NonQueryCommand>();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void InsertAffiliateInfoCommand_DefaultValues_ShouldBeZeroOrNull()
        {
            // Arrange & Act
            var command = new InsertAffiliateInfoCommand();

            // Assert
            command.CustId.Should().Be(0);
            command.SiteId.Should().Be(0);
            command.AffiliateId.Should().Be(0);
            command.AffiliateName.Should().BeNull();
            command.ParentAffiliate.Should().BeNull();
            command.AffiliateCode.Should().BeNull();
            command.UserName.Should().BeNull();
            command.FirstName.Should().BeNull();
            command.LastName.Should().BeNull();
            command.ContactNumber.Should().BeNull();
            command.Email.Should().BeNull();
            command.Status.Should().BeNull();
        }

        [Fact]
        public void InsertAffiliateInfoCommand_RealWorldScenario_ShouldWork()
        {
            // Arrange & Act
            var command = new InsertAffiliateInfoCommand
            {
                CustId = 12345,
                SiteId = 1,
                AffiliateId = 67890,
                AffiliateName = "Premium Affiliate Partner",
                ParentAffiliate = "Master Affiliate",
                AffiliateCode = "PREM001",
                UserName = "premium_user",
                FirstName = "John",
                LastName = "Smith",
                ContactNumber = "+1-555-123-4567",
                Email = "john.smith@premiumpartner.com",
                Status = "Active"
            };

            // Assert
            command.CustId.Should().Be(12345);
            command.SiteId.Should().Be(1);
            command.AffiliateId.Should().Be(67890);
            command.AffiliateName.Should().Be("Premium Affiliate Partner");
            command.ParentAffiliate.Should().Be("Master Affiliate");
            command.AffiliateCode.Should().Be("PREM001");
            command.UserName.Should().Be("premium_user");
            command.FirstName.Should().Be("John");
            command.LastName.Should().Be("Smith");
            command.ContactNumber.Should().Be("+1-555-123-4567");
            command.Email.Should().Be("john.smith@premiumpartner.com");
            command.Status.Should().Be("Active");
            command.GetSettingKey().Should().Be("Affiliate_AffiliateInfo_Set");
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertAffiliateInfoCommand_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var command1 = new InsertAffiliateInfoCommand { CustId = 1, AffiliateName = "Affiliate1" };
            var command2 = new InsertAffiliateInfoCommand { CustId = 2, AffiliateName = "Affiliate2" };

            // Act
            command1.CustId = 100;
            command1.AffiliateName = "Modified Affiliate1";

            // Assert
            command1.CustId.Should().Be(100);
            command1.AffiliateName.Should().Be("Modified Affiliate1");
            command2.CustId.Should().Be(2);
            command2.AffiliateName.Should().Be("Affiliate2");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void InsertAffiliateInfoCommand_AllPropertiesSet_ShouldMaintainValues()
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act - Set all properties
            command.CustId = 1;
            command.SiteId = 2;
            command.AffiliateId = 3;
            command.AffiliateName = "Test";
            command.ParentAffiliate = "Parent";
            command.AffiliateCode = "CODE";
            command.UserName = "user";
            command.FirstName = "First";
            command.LastName = "Last";
            command.ContactNumber = "123";
            command.Email = "test@test.com";
            command.Status = "Active";

            // Modify one property
            command.CustId = 999;

            // Assert - All other properties should remain unchanged
            command.CustId.Should().Be(999);
            command.SiteId.Should().Be(2);
            command.AffiliateId.Should().Be(3);
            command.AffiliateName.Should().Be("Test");
            command.ParentAffiliate.Should().Be("Parent");
            command.AffiliateCode.Should().Be("CODE");
            command.UserName.Should().Be("user");
            command.FirstName.Should().Be("First");
            command.LastName.Should().Be("Last");
            command.ContactNumber.Should().Be("123");
            command.Email.Should().Be("test@test.com");
            command.Status.Should().Be("Active");
        }

        [Fact]
        public void InsertAffiliateInfoCommand_MethodCallsMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();

            // Act
            var settingKey1 = command.GetSettingKey();
            var settingKey2 = command.GetSettingKey();
            var isValid1 = command.IsValid();
            var isValid2 = command.IsValid();

            // Assert
            settingKey1.Should().Be(settingKey2);
            settingKey1.Should().Be("Affiliate_AffiliateInfo_Set");
            isValid1.Should().Be(isValid2);
            isValid1.Should().BeTrue();
        }

        [Fact]
        public void InsertAffiliateInfoCommand_ExtremeStringValues_ShouldWork()
        {
            // Arrange
            var command = new InsertAffiliateInfoCommand();
            var longString = new string('A', 1000);

            // Act
            command.AffiliateName = longString;
            command.Email = longString;
            command.ContactNumber = longString;

            // Assert
            command.AffiliateName.Should().Be(longString);
            command.Email.Should().Be(longString);
            command.ContactNumber.Should().Be(longString);
            command.IsValid().Should().BeTrue();
        }

        #endregion
    }
}
