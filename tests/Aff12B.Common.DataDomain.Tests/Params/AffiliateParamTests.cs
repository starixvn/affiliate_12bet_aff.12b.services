using Aff12B.Common.DataDomain.Params;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Params
{
    /// <summary>
    /// Unit tests for AffiliateParam - Target: 95% Coverage
    /// </summary>
    public class AffiliateParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Act
            var param = new AffiliateParam();

            // Assert
            param.Should().NotBeNull();
            param.CustId.Should().Be(0);
            param.SiteId.Should().Be(0);
            param.AffiliateId.Should().Be(0);
            param.AffiliateName.Should().BeNull();
            param.ParentAffiliate.Should().BeNull();
            param.AffiliateCode.Should().BeNull();
            param.UserName.Should().BeNull();
            param.FirstName.Should().BeNull();
            param.LastName.Should().BeNull();
            param.ContactNumber.Should().BeNull();
            param.Email.Should().BeNull();
            param.Status.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.CustId = 12345;
            param.SiteId = 1;
            param.AffiliateId = 100;
            param.AffiliateName = "Test Affiliate";
            param.ParentAffiliate = "Parent Affiliate";
            param.AffiliateCode = "AFF001";
            param.UserName = "testaffiliate";
            param.FirstName = "Test";
            param.LastName = "Affiliate";
            param.ContactNumber = "+1234567890";
            param.Email = "test@affiliate.com";
            param.Status = "Active";

            // Assert
            param.CustId.Should().Be(12345);
            param.SiteId.Should().Be(1);
            param.AffiliateId.Should().Be(100);
            param.AffiliateName.Should().Be("Test Affiliate");
            param.ParentAffiliate.Should().Be("Parent Affiliate");
            param.AffiliateCode.Should().Be("AFF001");
            param.UserName.Should().Be("testaffiliate");
            param.FirstName.Should().Be("Test");
            param.LastName.Should().Be("Affiliate");
            param.ContactNumber.Should().Be("+1234567890");
            param.Email.Should().Be("test@affiliate.com");
            param.Status.Should().Be("Active");
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(12345)]
        [InlineData(int.MaxValue)]
        public void IntegerProperties_ShouldHandleValues(int value)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act & Assert - CustId
            param.CustId = value;
            param.CustId.Should().Be(value);

            // Act & Assert - SiteId
            param.SiteId = value;
            param.SiteId.Should().Be(value);

            // Act & Assert - AffiliateId
            param.AffiliateId = value;
            param.AffiliateId.Should().Be(value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Test Affiliate")]
        [InlineData("Very Long Affiliate Name With Special Characters 123!@#")]
        public void AffiliateName_ShouldHandleStringValues(string affiliateName)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.AffiliateName = affiliateName;

            // Assert
            param.AffiliateName.Should().Be(affiliateName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Parent Affiliate")]
        [InlineData("Very Long Parent Affiliate Name")]
        public void ParentAffiliate_ShouldHandleStringValues(string parentAffiliate)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.ParentAffiliate = parentAffiliate;

            // Assert
            param.ParentAffiliate.Should().Be(parentAffiliate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("AFF001")]
        [InlineData("AFFILIATE_CODE_123")]
        [InlineData("VERY_LONG_AFFILIATE_CODE_WITH_UNDERSCORES")]
        public void AffiliateCode_ShouldHandleStringValues(string affiliateCode)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.AffiliateCode = affiliateCode;

            // Assert
            param.AffiliateCode.Should().Be(affiliateCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("testuser")]
        [InlineData("very_long_username_with_special_characters")]
        public void UserName_ShouldHandleStringValues(string userName)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.UserName = userName;

            // Assert
            param.UserName.Should().Be(userName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("John")]
        [InlineData("Very Long First Name")]
        public void FirstName_ShouldHandleStringValues(string firstName)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.FirstName = firstName;

            // Assert
            param.FirstName.Should().Be(firstName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Doe")]
        [InlineData("Very Long Last Name")]
        public void LastName_ShouldHandleStringValues(string lastName)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.LastName = lastName;

            // Assert
            param.LastName.Should().Be(lastName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("+1234567890")]
        [InlineData("123-456-7890")]
        [InlineData("(123) 456-7890")]
        [InlineData("123.456.7890")]
        public void ContactNumber_ShouldHandlePhoneFormats(string contactNumber)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.ContactNumber = contactNumber;

            // Assert
            param.ContactNumber.Should().Be(contactNumber);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("test@example.com")]
        [InlineData("very.long.email.address@example.com")]
        [InlineData("user+tag@example.co.uk")]
        public void Email_ShouldHandleEmailFormats(string email)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.Email = email;

            // Assert
            param.Email.Should().Be(email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Active")]
        [InlineData("Inactive")]
        [InlineData("Pending")]
        [InlineData("Suspended")]
        public void Status_ShouldHandleStatusValues(string status)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.Status = status;

            // Assert
            param.Status.Should().Be(status);
        }

        [Fact]
        public void AffiliateParam_ShouldHandleCompleteAffiliateInfo()
        {
            // Arrange & Act
            var param = new AffiliateParam
            {
                CustId = 12345,
                SiteId = 1,
                AffiliateId = 100,
                AffiliateName = "Premium Affiliate",
                ParentAffiliate = "Master Affiliate",
                AffiliateCode = "PREM001",
                UserName = "premiumaff",
                FirstName = "Premium",
                LastName = "Affiliate",
                ContactNumber = "+1-555-123-4567",
                Email = "premium@affiliate.com",
                Status = "Active"
            };

            // Assert
            param.CustId.Should().Be(12345);
            param.SiteId.Should().Be(1);
            param.AffiliateId.Should().Be(100);
            param.AffiliateName.Should().Be("Premium Affiliate");
            param.ParentAffiliate.Should().Be("Master Affiliate");
            param.AffiliateCode.Should().Be("PREM001");
            param.UserName.Should().Be("premiumaff");
            param.FirstName.Should().Be("Premium");
            param.LastName.Should().Be("Affiliate");
            param.ContactNumber.Should().Be("+1-555-123-4567");
            param.Email.Should().Be("premium@affiliate.com");
            param.Status.Should().Be("Active");
        }

        [Fact]
        public void AffiliateParam_ShouldHandleMinimalInfo()
        {
            // Arrange & Act
            var param = new AffiliateParam
            {
                CustId = 1,
                SiteId = 1,
                AffiliateId = 1
            };

            // Assert
            param.CustId.Should().Be(1);
            param.SiteId.Should().Be(1);
            param.AffiliateId.Should().Be(1);
            param.AffiliateName.Should().BeNull();
            param.ParentAffiliate.Should().BeNull();
            param.AffiliateCode.Should().BeNull();
            param.UserName.Should().BeNull();
            param.FirstName.Should().BeNull();
            param.LastName.Should().BeNull();
            param.ContactNumber.Should().BeNull();
            param.Email.Should().BeNull();
            param.Status.Should().BeNull();
        }

        [Fact]
        public void AffiliateParam_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var param = new AffiliateParam();
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            // Act
            param.AffiliateName = specialChars;
            param.ParentAffiliate = specialChars;
            param.AffiliateCode = specialChars;
            param.UserName = specialChars;
            param.FirstName = specialChars;
            param.LastName = specialChars;
            param.ContactNumber = specialChars;
            param.Email = specialChars;
            param.Status = specialChars;

            // Assert
            param.AffiliateName.Should().Be(specialChars);
            param.ParentAffiliate.Should().Be(specialChars);
            param.AffiliateCode.Should().Be(specialChars);
            param.UserName.Should().Be(specialChars);
            param.FirstName.Should().Be(specialChars);
            param.LastName.Should().Be(specialChars);
            param.ContactNumber.Should().Be(specialChars);
            param.Email.Should().Be(specialChars);
            param.Status.Should().Be(specialChars);
        }

        [Fact]
        public void AffiliateParam_ShouldHandleEmptyStrings()
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.AffiliateName = "";
            param.ParentAffiliate = "";
            param.AffiliateCode = "";
            param.UserName = "";
            param.FirstName = "";
            param.LastName = "";
            param.ContactNumber = "";
            param.Email = "";
            param.Status = "";

            // Assert
            param.AffiliateName.Should().Be("");
            param.ParentAffiliate.Should().Be("");
            param.AffiliateCode.Should().Be("");
            param.UserName.Should().Be("");
            param.FirstName.Should().Be("");
            param.LastName.Should().Be("");
            param.ContactNumber.Should().Be("");
            param.Email.Should().Be("");
            param.Status.Should().Be("");
        }

        [Fact]
        public void AffiliateParam_ShouldHandleWhitespaceStrings()
        {
            // Arrange
            var param = new AffiliateParam();
            var whitespace = "   ";

            // Act
            param.AffiliateName = whitespace;
            param.ParentAffiliate = whitespace;
            param.AffiliateCode = whitespace;
            param.UserName = whitespace;
            param.FirstName = whitespace;
            param.LastName = whitespace;
            param.ContactNumber = whitespace;
            param.Email = whitespace;
            param.Status = whitespace;

            // Assert
            param.AffiliateName.Should().Be(whitespace);
            param.ParentAffiliate.Should().Be(whitespace);
            param.AffiliateCode.Should().Be(whitespace);
            param.UserName.Should().Be(whitespace);
            param.FirstName.Should().Be(whitespace);
            param.LastName.Should().Be(whitespace);
            param.ContactNumber.Should().Be(whitespace);
            param.Email.Should().Be(whitespace);
            param.Status.Should().Be(whitespace);
        }
    }
}
