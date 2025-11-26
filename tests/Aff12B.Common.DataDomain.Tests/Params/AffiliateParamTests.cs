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
            param.SiteId.Should().Be(0);
            param.AffId.Should().Be(0);
            param.ParentAff.Should().Be(string.Empty); // Default value
            param.UserName.Should().BeNull();
            param.Player_CustId.Should().Be(0);
            param.CustId.Should().Be(-1);
            param.Player_Username.Should().Be(string.Empty); // Default value
            param.FirstName.Should().Be(string.Empty); // Default value
            param.LastName.Should().Be(string.Empty); // Default value
            param.Phone.Should().Be(string.Empty); // Default value
            param.Email.Should().Be(string.Empty); // Default value
            param.Status.Should().Be(string.Empty); // Default value
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.Player_CustId = 12345;
            param.SiteId = 1;
            param.AffId = 100;
            param.UserName = "Test Affiliate";
            param.ParentAff = "Parent Affiliate";
            param.AffCode = "AFF001";
            param.Player_Username = "testaffiliate";
            param.FirstName = "Test";
            param.LastName = "Affiliate";
            param.Phone = "+1234567890";
            param.Email = "test@affiliate.com";
            param.Status = "Active";

            // Assert
            param.CustId.Should().Be(12345);
            param.SiteId.Should().Be(1);
            param.AffId.Should().Be(100);
            param.UserName.Should().Be("Test Affiliate");
            param.ParentAff.Should().Be("Parent Affiliate");
            param.AffCode.Should().Be("AFF001");
            param.Player_Username.Should().Be("testaffiliate");
            param.FirstName.Should().Be("Test");
            param.LastName.Should().Be("Affiliate");
            param.Phone.Should().Be("+1234567890");
            param.Email.Should().Be("test@affiliate.com");
            param.Status.Should().Be("Active");
        }

        [Theory]
        [InlineData(int.MinValue, -1)]
        [InlineData(-1, -1)]
        [InlineData(0, -1)]
        [InlineData(1, 1)]
        [InlineData(12345, 12345)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public void IntegerProperties_ShouldHandleValues(int value, int expectedCustId)
        {
            // Arrange
            var param = new AffiliateParam();

            // Act & Assert - CustId
            param.Player_CustId = value;
            param.CustId.Should().Be(expectedCustId);

            // Act & Assert - SiteId
            param.SiteId = value;
            param.SiteId.Should().Be(value);

            // Act & Assert - AffiliateId
            param.AffId = value;
            param.AffId.Should().Be(value);
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
            param.UserName = affiliateName;

            // Assert
            param.UserName.Should().Be(affiliateName);
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
            param.ParentAff = parentAffiliate;

            // Assert
            param.ParentAff.Should().Be(parentAffiliate);
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
            param.AffCode = affiliateCode;

            // Assert
            param.AffCode.Should().Be(affiliateCode);
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
            param.Phone = contactNumber;

            // Assert
            param.Phone.Should().Be(contactNumber);
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
                Player_CustId = 12345,
                SiteId = 1,
                AffId = 100,
                UserName = "Premium Affiliate",
                ParentAff = "Master Affiliate",
                AffCode = "PREM001",
                Player_Username = "premiumaff",
                FirstName = "Premium",
                LastName = "Affiliate",
                Phone = "+1-555-123-4567",
                Email = "premium@affiliate.com",
                Status = "Active"
            };

            // Assert
            param.CustId.Should().Be(12345);
            param.SiteId.Should().Be(1);
            param.AffId.Should().Be(100);
            param.UserName.Should().Be("Premium Affiliate");
            param.ParentAff.Should().Be("Master Affiliate");
            param.AffCode.Should().Be("PREM001");
            param.Player_Username.Should().Be("premiumaff");
            param.FirstName.Should().Be("Premium");
            param.LastName.Should().Be("Affiliate");
            param.Phone.Should().Be("+1-555-123-4567");
            param.Email.Should().Be("premium@affiliate.com");
            param.Status.Should().Be("Active");
        }

        [Fact]
        public void AffiliateParam_ShouldHandleMinimalInfo()
        {
            // Arrange & Act
            var param = new AffiliateParam
            {
                Player_CustId = 1,
                SiteId = 1,
                AffId = 1
            };

            // Assert
            param.SiteId.Should().Be(1);
            param.AffId.Should().Be(1);
            param.ParentAff.Should().Be(string.Empty); // Default value
            param.UserName.Should().BeNull();
            param.Player_CustId.Should().Be(1);
            param.CustId.Should().Be(1);
            param.Player_Username.Should().Be(string.Empty); // Default value
            param.FirstName.Should().Be(string.Empty); // Default value
            param.LastName.Should().Be(string.Empty); // Default value
            param.Phone.Should().Be(string.Empty); // Default value
            param.Email.Should().Be(string.Empty); // Default value
            param.Status.Should().Be(string.Empty); // Default value
        }

        [Fact]
        public void AffiliateParam_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var param = new AffiliateParam();
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            // Act
            param.UserName = specialChars;
            param.ParentAff = specialChars;
            param.AffCode = specialChars;
            param.Player_Username = specialChars;
            param.FirstName = specialChars;
            param.LastName = specialChars;
            param.Phone = specialChars;
            param.Email = specialChars;
            param.Status = specialChars;

            // Assert
            param.UserName.Should().Be(specialChars);
            param.ParentAff.Should().Be(specialChars);
            param.AffCode.Should().Be(specialChars);
            param.Player_Username.Should().Be(specialChars);
            param.FirstName.Should().Be(specialChars);
            param.LastName.Should().Be(specialChars);
            param.Phone.Should().Be(specialChars);
            param.Email.Should().Be(specialChars);
            param.Status.Should().Be(specialChars);
        }

        [Fact]
        public void AffiliateParam_ShouldHandleEmptyStrings()
        {
            // Arrange
            var param = new AffiliateParam();

            // Act
            param.UserName = "";
            param.ParentAff = "";
            param.AffCode = "";
            param.Player_Username = "";
            param.FirstName = "";
            param.LastName = "";
            param.Phone = "";
            param.Email = "";
            param.Status = "";

            // Assert
            param.UserName.Should().Be("");
            param.ParentAff.Should().Be("");
            param.AffCode.Should().Be("");
            param.Player_Username.Should().Be("");
            param.FirstName.Should().Be("");
            param.LastName.Should().Be("");
            param.Phone.Should().Be("");
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
            param.UserName = whitespace;
            param.ParentAff = whitespace;
            param.AffCode = whitespace;
            param.Player_Username = whitespace;
            param.FirstName = whitespace;
            param.LastName = whitespace;
            param.Phone = whitespace;
            param.Email = whitespace;
            param.Status = whitespace;

            // Assert
            param.UserName.Should().Be(whitespace);
            param.ParentAff.Should().Be(whitespace);
            param.AffCode.Should().Be(whitespace);
            param.Player_Username.Should().Be(whitespace);
            param.FirstName.Should().Be(whitespace);
            param.LastName.Should().Be(whitespace);
            param.Phone.Should().Be(whitespace);
            param.Email.Should().Be(whitespace);
            param.Status.Should().Be(whitespace);
        }
    }
}
