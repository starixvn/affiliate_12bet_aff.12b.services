using FluentAssertions;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Extensions
{
    /// <summary>
    /// Unit tests for HashExtensions class
    /// Tests the SHA256 hashing functionality with various inputs
    /// Target: 100% code coverage
    /// </summary>
    public class HashExtensionsTests
    {
        [Fact]
        public void Sha256_ValidString_ShouldReturnBase64Hash()
        {
            // Arrange
            var input = "test";
            var expectedHash = ComputeExpectedSha256Hash(input);

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Be(expectedHash);
        }

        [Fact]
        public void Sha256_EmptyString_ShouldReturnEmpty()
        {
            // Arrange
            var input = "";

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void Sha256_NullString_ShouldReturnEmpty()
        {
            // Arrange
            string input = null;

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void Sha256_WhitespaceString_ShouldReturnEmpty()
        {
            // Arrange
            var input = "   ";

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().Be(string.Empty);
        }

        [Theory]
        [InlineData("password", "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=")]
        [InlineData("12345", "WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=")]
        [InlineData("admin", "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=")]
        [InlineData("test123", "7NcYcNGWMxapfjrDQIyYNa2M8PPBvHA1J8MCZVNPda4=")]
        public void Sha256_KnownInputs_ShouldReturnExpectedHashes(string input, string expectedHash)
        {
            // Act
            var result = input.Sha256();

            // Assert
            result.Should().Be(expectedHash);
        }

        [Theory]
        [InlineData("测试", "Chinese characters")]
        [InlineData("тест", "Cyrillic characters")]
        [InlineData("🔒🔑", "Emoji characters")]
        [InlineData("café", "Accented characters")]
        [InlineData("αβγδε", "Greek characters")]
        public void Sha256_UnicodeStrings_ShouldHandleCorrectly(string input, string description)
        {
            // Act
            var result = input.Sha256();

            // Assert
            result.Should().NotBeNullOrEmpty($"should handle {description}");
            result.Should().MatchRegex("^[A-Za-z0-9+/]+=*$", "should be valid base64");

            // Verify it's consistent
            var secondResult = input.Sha256();
            result.Should().Be(secondResult, "should be consistent for same input");
        }

        [Fact]
        public void Sha256_LargeString_ShouldProcessCorrectly()
        {
            // Arrange
            var largeString = new string('A', 10000); // 10KB string

            // Act
            var result = largeString.Sha256();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9+/]+=*$", "should be valid base64");

            // Verify consistency
            var secondResult = largeString.Sha256();
            result.Should().Be(secondResult);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("\r\n")]
        [InlineData(" \t \n ")]
        public void Sha256_WhitespaceVariations_ShouldReturnEmpty(string input)
        {
            // Act
            var result = input.Sha256();

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void Sha256_SpecialCharacters_ShouldProcessCorrectly()
        {
            // Arrange
            var input = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9+/]+=*$");
        }

        [Fact]
        public void Sha256_ConsistentResults_ShouldReturnSameHashForSameInput()
        {
            // Arrange
            var input = "consistent_test";

            // Act
            var result1 = input.Sha256();
            var result2 = input.Sha256();
            var result3 = input.Sha256();

            // Assert
            result1.Should().Be(result2);
            result2.Should().Be(result3);
            result1.Should().Be(result3);
        }

        [Fact]
        public void Sha256_DifferentInputs_ShouldReturnDifferentHashes()
        {
            // Arrange
            var input1 = "test1";
            var input2 = "test2";

            // Act
            var result1 = input1.Sha256();
            var result2 = input2.Sha256();

            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void Sha256_CaseSensitive_ShouldReturnDifferentHashesForDifferentCase()
        {
            // Arrange
            var input1 = "Test";
            var input2 = "test";

            // Act
            var result1 = input1.Sha256();
            var result2 = input2.Sha256();

            // Assert
            result1.Should().NotBe(result2);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("ab")]
        [InlineData("abc")]
        [InlineData("abcd")]
        [InlineData("abcde")]
        public void Sha256_VariousLengths_ShouldProcessCorrectly(string input)
        {
            // Act
            var result = input.Sha256();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9+/]+=*$");
        }

        [Fact]
        public void Sha256_NumericString_ShouldProcessCorrectly()
        {
            // Arrange
            var input = "1234567890";

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9+/]+=*$");
        }

        /// <summary>
        /// Helper method to compute expected SHA256 hash for verification
        /// </summary>
        private static string ComputeExpectedSha256Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
