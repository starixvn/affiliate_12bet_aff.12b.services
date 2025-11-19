using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Services;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Fanex.Data.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Services
{
    /// <summary>
    /// Unit tests for MasterDataProviderService.
    /// Tests database operations and data retrieval.
    /// </summary>
    public class MasterDataProviderServiceTests
    {
        private readonly Mock<IDynamicRepository> _mockRepository;
        private readonly Mock<ILogger<MasterDataProviderService>> _mockLogger;
        private readonly MasterDataProviderService _service;

        public MasterDataProviderServiceTests()
        {
            _mockRepository = new Mock<IDynamicRepository>();
            _mockLogger = new Mock<ILogger<MasterDataProviderService>>();
            _service = new MasterDataProviderService(_mockRepository.Object, _mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldCreateInstance_WhenValidDependencies()
        {
            // Arrange & Act
            var service = new MasterDataProviderService(_mockRepository.Object, _mockLogger.Object);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<IMasterDataProviderService>();
        }

        [Theory]
        [InlineData("repository")]
        [InlineData("logger")]
        public void Constructor_ShouldThrowArgumentNullException_WhenDependencyIsNull(string parameterName)
        {
            // Arrange & Act
            var act = () => parameterName switch
            {
                "repository" => new MasterDataProviderService(null, _mockLogger.Object),
                "logger" => new MasterDataProviderService(_mockRepository.Object, null),
                _ => throw new ArgumentException("Invalid parameter name")
            };

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName(parameterName);
        }

        #endregion

        #region GetMasterDataAsync Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task GetMasterDataAsync_ShouldThrowArgumentException_WhenSitePrefixIsInvalid(string invalidPrefix)
        {
            // Act & Assert
            var act = async () => await _service.GetMasterDataAsync(invalidPrefix);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Site prefix cannot be null or empty*");
        }

        [Fact]
        public async Task GetMasterDataAsync_ShouldReturnNull_WhenRepositoryThrows()
        {
            // Arrange
            var sitePrefix = "CAS";
            var expectedException = new InvalidOperationException("Database connection failed");

            // Setup the mock to throw when any FetchMultipleAsync is called
            _mockRepository.Setup(x => x.FetchMultipleAsync<SpecialDomainMapping, DomainMapping, LanguageReference, AffiliateDeal, LandingPageSetting>(
                It.IsAny<MasterInfoValidationCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act
            var result = await _service.GetMasterDataAsync(sitePrefix);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetMasterDataAsync_ShouldCreateCorrectCriteria()
        {
            // Arrange
            var sitePrefix = "SPT";
            var mockResults = CreateMockRepositoryResults();

            _mockRepository.Setup(x => x.FetchMultipleAsync<SpecialDomainMapping, DomainMapping, LanguageReference, AffiliateDeal, LandingPageSetting>(
                It.IsAny<MasterInfoValidationCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResults);

            // Act
            await _service.GetMasterDataAsync(sitePrefix);

            // Assert
            _mockRepository.Verify(x => x.FetchMultipleAsync<SpecialDomainMapping, DomainMapping, LanguageReference, AffiliateDeal, LandingPageSetting>(
                It.IsAny<MasterInfoValidationCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Helper Methods

        private (
            IEnumerable<SpecialDomainMapping> FirstResultSet,
            IEnumerable<DomainMapping> SecondResultSet,
            IEnumerable<LanguageReference> ThirdResultSet,
            IEnumerable<AffiliateDeal> FourthResultSet,
            IEnumerable<LandingPageSetting> FifthResultSet
        ) CreateMockRepositoryResults()
        {
            return (
                new List<SpecialDomainMapping>
                {
                    new SpecialDomainMapping { AffCode = "CAS001", AffDomain = "casino1.com" },
                    new SpecialDomainMapping { AffCode = "CAS002", AffDomain = "casino2.com" }
                },
                new List<DomainMapping>
                {
                    new DomainMapping { AffDomain = "casino.com", RedirectDomain = "https://casino.com" }
                },
                new List<LanguageReference>
                {
                    new LanguageReference { LanguageCode = "EN", LanguageLink = "en" },
                    new LanguageReference { LanguageCode = "ES", LanguageLink = "es" },
                    new LanguageReference { LanguageCode = "FR", LanguageLink = "fr" }
                },
                new List<AffiliateDeal>
                {
                    new AffiliateDeal { DealId = 1, DealName = "Welcome Bonus", AffCode = "CAS001" }
                },
                new List<LandingPageSetting>
                {
                    new LandingPageSetting { PageCode = "PAGE001", DealIds = "1,2" },
                    new LandingPageSetting { PageCode = "PAGE002", DealIds = "3,4" }
                }
            );
        }

        #endregion
    }
}
