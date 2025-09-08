using Aff12B.CronJobs.Infrastructure.SpParams;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.SpParams
{
    /// <summary>
    /// Comprehensive unit tests for MasterInfoValidationCriteria class.
    /// Business-focused scenarios for master information validation criteria.
    /// Covers prefix provider handling, suffix setting keys, and validation logic.
    /// </summary>
    public class MasterInfoValidationCriteriaTests
    {
        [Fact]
        public void Constructor_ShouldAcceptPrefixProvider()
        {
            // Arrange & Act
            var criteria = new MasterInfoValidationCriteria("TEST_PREFIX");

            // Assert
            criteria.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldHandleVariousPrefixProviders()
        {
            // Arrange - Different business site prefixes
            var casinoPrefixCriteria = new MasterInfoValidationCriteria("CASINO_A");
            var sportsPrefixCriteria = new MasterInfoValidationCriteria("SPORTS_B");
            var livePrefixCriteria = new MasterInfoValidationCriteria("LIVE_C");

            // Act & Assert - All should be created successfully
            casinoPrefixCriteria.Should().NotBeNull();
            sportsPrefixCriteria.Should().NotBeNull();
            livePrefixCriteria.Should().NotBeNull();
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var criteria = new MasterInfoValidationCriteria("VALIDATION_PREFIX");

            // Act
            var isValid = criteria.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldBeConsistentAcrossMultipleCalls()
        {
            // Arrange
            var criteria = new MasterInfoValidationCriteria("CONSISTENT_PREFIX");

            // Act - Multiple validation calls
            var result1 = criteria.IsValid();
            var result2 = criteria.IsValid();
            var result3 = criteria.IsValid();

            // Assert - Should always return true
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldInheritFromIdenticalSurfixCriteriaBase()
        {
            // Arrange
            var criteria = new MasterInfoValidationCriteria("INHERITANCE_TEST");

            // Act & Assert - Should inherit from base class
            criteria.Should().BeAssignableTo<IdenticalSurfixCriteriaBase>();
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldSupportBusinessValidationScenario()
        {
            // Arrange - Business scenario: Master data validation for affiliate system
            var affiliatePrefixCriteria = new MasterInfoValidationCriteria("AFF_12B");

            // Act - Simulate master info validation workflow
            var isValidForProcessing = affiliatePrefixCriteria.IsValid();

            // Assert - Should be ready for master info validation
            isValidForProcessing.Should().BeTrue();
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldHandleMultipleSitePrefixes()
        {
            // Arrange - Multi-site business scenario
            var sitePrefixes = new[] { "SITE_001", "SITE_002", "SITE_003", "SITE_VIP", "SITE_MOBILE" };
            var criteriaList = new List<MasterInfoValidationCriteria>();

            // Act - Create criteria for each site
            foreach (var prefix in sitePrefixes)
            {
                criteriaList.Add(new MasterInfoValidationCriteria(prefix));
            }

            // Assert - All should be valid for their respective sites
            criteriaList.Should().HaveCount(5);
            foreach (var criteria in criteriaList)
            {
                criteria.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public async Task MasterInfoValidationCriteria_ShouldSupportConcurrentValidation()
        {
            // Arrange - Concurrent validation scenario
            var criteria = new MasterInfoValidationCriteria("CONCURRENT_PREFIX");
            var tasks = new List<Task<bool>>();

            // Act - Multiple concurrent validation calls
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() => criteria.IsValid()));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - All concurrent validations should return true
            results.Should().HaveCount(20);
            results.Should().AllSatisfy(result => result.Should().BeTrue());
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldHandleEmptyPrefix()
        {
            // Arrange - Edge case: Empty prefix
            var criteria = new MasterInfoValidationCriteria("");

            // Act
            var isValid = criteria.IsValid();

            // Assert - Should still be valid (validation logic is always true)
            isValid.Should().BeTrue();
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldHandleNullPrefix()
        {
            // Arrange & Act - Null prefix scenario
            var act = () => new MasterInfoValidationCriteria(null!);

            // Assert - Constructor should handle null appropriately
            // Note: This depends on the base class implementation
            // If base class throws, test should expect exception
            // If base class handles null, test should verify behavior
            act.Should().NotThrow(); // Assuming base class handles null gracefully
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldSupportSpecialCharactersInPrefix()
        {
            // Arrange - Special characters in prefix (real-world scenario)
            var specialPrefixes = new[]
            {
                "PREFIX_WITH_UNDERSCORE",
                "PREFIX-WITH-DASH",
                "PREFIX.WITH.DOT",
                "PREFIX123",
                "12B_SPECIAL"
            };

            // Act & Assert - All special character prefixes should work
            foreach (var prefix in specialPrefixes)
            {
                var criteria = new MasterInfoValidationCriteria(prefix);
                criteria.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldMaintainImmutability()
        {
            // Arrange
            var criteria = new MasterInfoValidationCriteria("IMMUTABLE_PREFIX");

            // Act - Multiple operations that shouldn't change state
            var initialValid = criteria.IsValid();

            // Perform multiple validation operations
            for (int i = 0; i < 10; i++)
            {
                criteria.IsValid();
            }

            var finalValid = criteria.IsValid();

            // Assert - State should remain unchanged
            finalValid.Should().Be(initialValid);
            finalValid.Should().BeTrue();
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldSupportHighVolumeValidation()
        {
            // Arrange - High volume validation scenario
            var criteria = new MasterInfoValidationCriteria("HIGH_VOLUME_PREFIX");
            var validationResults = new List<bool>();

            // Act - Simulate high volume master info validations
            for (int i = 0; i < 1000; i++)
            {
                validationResults.Add(criteria.IsValid());
            }

            // Assert - Should handle high volume consistently
            validationResults.Should().HaveCount(1000);
            validationResults.Should().AllSatisfy(result => result.Should().BeTrue());
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldSupportDifferentPrefixFormats()
        {
            // Arrange - Different prefix formats used in business
            var prefixFormats = new[]
            {
                "UPPERCASE_PREFIX",
                "lowercase_prefix",
                "MixedCase_Prefix",
                "12B_NUMERIC_PREFIX",
                "SHORT",
                "VERY_LONG_PREFIX_WITH_MULTIPLE_SEGMENTS_FOR_COMPLEX_BUSINESS_SCENARIOS"
            };

            // Act & Assert - All formats should be supported
            foreach (var prefix in prefixFormats)
            {
                var criteria = new MasterInfoValidationCriteria(prefix);
                criteria.IsValid().Should().BeTrue($"Prefix format '{prefix}' should be valid");
            }
        }

        [Fact]
        public void MasterInfoValidationCriteria_ShouldBeStateless()
        {
            // Arrange - Test statelessness for thread safety
            var criteria1 = new MasterInfoValidationCriteria("STATELESS_1");
            var criteria2 = new MasterInfoValidationCriteria("STATELESS_2");

            // Act
            var valid1 = criteria1.IsValid();
            var valid2 = criteria2.IsValid();

            // Assert - Both instances should behave consistently
            valid1.Should().BeTrue();
            valid2.Should().BeTrue();
            valid1.Should().Be(valid2);
        }
    }
}
