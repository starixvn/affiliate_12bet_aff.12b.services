using Aff12B.Common.DataDomain.Models;
using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aff12B.Common.Tests
{
    /// <summary>
    /// Shared test utilities and helper methods for all test projects
    /// </summary>
    public static class TestUtilities
    {
        /// <summary>
        /// Creates a mock logger for the specified type
        /// </summary>
        /// <typeparam name="T">Type for which to create the logger</typeparam>
        /// <returns>Mock logger instance</returns>
        public static Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        /// <summary>
        /// Creates an AutoFixture instance with common customizations optimized for memory usage
        /// </summary>
        /// <returns>Configured AutoFixture instance</returns>
        public static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            // Customize for common scenarios
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(recursionDepth: 1)); // Limit recursion depth

            // Set up reasonable defaults for nullable DateTime with fixed seed to reduce memory
            var random = new Random(42); // Fixed seed for consistent, memory-efficient generation
            fixture.Customize<DateTime?>(composer => composer
                .FromFactory(() => DateTime.UtcNow.AddDays(random.Next(-30, 30))));

            // Limit string generation to reduce memory usage
            fixture.Customize<string>(composer => composer
                .FromFactory(() => $"Test{random.Next(1, 1000)}"));

            return fixture;
        }

        /// <summary>
        /// Creates a test TokenManagement configuration
        /// </summary>
        /// <returns>TokenManagement instance for testing</returns>
        public static TokenManagement CreateTestTokenManagement()
        {
            return new TokenManagement
            {
                Secret = "test-secret-key-for-jwt-token-generation-minimum-256-bits",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessExpiration = 60 // 60 minutes
            };
        }

        /// <summary>
        /// Creates a list of test users for authentication testing
        /// </summary>
        /// <returns>List of test users</returns>
        public static List<User> CreateTestUsers()
        {
            return new List<User>
            {
                new User
                {
                    UserName = "testuser",
                    Password = "5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8" // "password" SHA256 hash
                },
                new User
                {
                    UserName = "admin",
                    Password = "EF92B778BAFE771E89245B89ECBC08A44A4E166C06659911881F383D4473E94F" // "secret123" SHA256 hash
                }
            };
        }

        /// <summary>
        /// Creates a test customer model with default values
        /// </summary>
        /// <returns>CustomerModel instance for testing</returns>
        public static CustomerModel CreateTestCustomerModel()
        {
            return new CustomerModel
            {
                CustId = 12345,
                UserName = "testuser",
                RegTime = DateTime.UtcNow.AddDays(-30),
                IsTest = false,
                TagList = "tag1,tag2,tag3"
            };
        }

        /// <summary>
        /// Creates a test affiliate customer model with default values
        /// </summary>
        /// <returns>AffCustomerModel instance for testing</returns>
        public static AffCustomerModel CreateTestAffCustomerModel()
        {
            return new AffCustomerModel
            {
                CustId = 12345,
                UserName = "testuser",
                ClickId = 98765,
                FirstDepositDate = DateTime.UtcNow.AddDays(-15),
                RegTime = DateTime.UtcNow.AddDays(-30)
            };
        }

        /// <summary>
        /// Verifies that a mock logger was called with the expected log level
        /// </summary>
        /// <typeparam name="T">Logger type</typeparam>
        /// <param name="mockLogger">Mock logger instance</param>
        /// <param name="logLevel">Expected log level</param>
        /// <param name="times">Number of times the log should have been called</param>
        public static void VerifyLoggerCalled<T>(Mock<ILogger<T>> mockLogger, LogLevel logLevel, Times times)
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times);
        }

        /// <summary>
        /// Creates a test bonus model with default values
        /// </summary>
        /// <returns>BonusModel instance for testing</returns>
        public static BonusModel CreateTestBonusModel()
        {
            return new BonusModel
            {
                BonusId = 1,
                TransId = "BONUS_001",
                PromotionType = 1,
                BonusCode = "WELCOME100",
                CustId = 12345,
                UserName = "testuser",
                CurrencyId = 1,
                SiteId = 1,
                LockedAmount = 100.00m,
                BonusAmount = 50.00m,
                ExpiryTime = DateTime.UtcNow.AddDays(30),
                BonusInfo = "Test bonus",
                Memo = "Test bonus memo",
                CreateTime = DateTime.UtcNow,
                Status = 1
            };
        }

        /// <summary>
        /// Creates a test bet record model with default values
        /// </summary>
        /// <returns>TpBetRecordModel instance for testing</returns>
        public static TpBetRecordModel CreateTestTpBetRecordModel()
        {
            return new TpBetRecordModel
            {
                TpId = 1,
                TransId = "BET_001",
                GameType = 1,
                CustId = "12345",
                UserName = "testuser",
                CurrencyId = 1,
                SiteId = 1,
                TpUserName = "tp_testuser",
                TransTime = DateTime.UtcNow.AddHours(-2),
                SettledTime = DateTime.UtcNow.AddHours(-1),
                Stake = 100.00m,
                WinlostAmount = 50.00m,
                SettlementType = 1,
                TicketStatus = 1,
                OriginalState = "SETTLED",
                TicketKey = 123456,
                CreateTime = DateTime.UtcNow.AddHours(-2),
                ValidBet = 100.00m,
                Revenue = 50.00m
            };
        }

        /// <summary>
        /// Creates a test tag model with default values
        /// </summary>
        /// <returns>TagModel instance for testing</returns>
        public static TagModel CreateTestTagModel()
        {
            return new TagModel
            {
                Id = 1,
                TagName = "test-tag",
                Description = "Test tag description",
                Status = 1,
                UpdateTime = DateTime.UtcNow,
                IsAbnormal = 0
            };
        }

        /// <summary>
        /// Creates a date range for testing date-based queries
        /// </summary>
        /// <param name="daysBack">Number of days back from today for start date</param>
        /// <param name="daysForward">Number of days forward from start date for end date</param>
        /// <returns>Tuple containing from and to dates</returns>
        public static (DateTime From, DateTime To) CreateDateRange(int daysBack = 7, int daysForward = 1)
        {
            var from = DateTime.UtcNow.AddDays(-daysBack).Date;
            var to = from.AddDays(daysForward);
            return (from, to);
        }
    }
}
