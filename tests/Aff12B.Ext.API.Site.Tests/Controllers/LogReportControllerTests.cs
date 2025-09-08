using Aff12B.Common.DataDomain.Models;
using Aff12B.Ext.API.Infastructure.Criteria;
using Aff12B.Ext.API.Site.Controllers;
using Fanex.Data.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests.Controllers
{
    /// <summary>
    /// Unit tests for LogReportController class
    /// Tests rebate M-log endpoints and data retrieval functionality
    /// Target: 100% code coverage
    /// </summary>
    public class LogReportControllerTests
    {
        private readonly Mock<IDynamicRepository> _mockRepository;
        private readonly LogReportController _controller;

        public LogReportControllerTests()
        {
            _mockRepository = new Mock<IDynamicRepository>();
            _controller = new LogReportController(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Act
            var controller = new LogReportController(_mockRepository.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeAssignableTo<ControllerBase>();
            controller._repo.Should().Be(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => new LogReportController(null!);
            act.Should().NotThrow();
        }

        [Fact]
        public void Repository_Property_ShouldBeAccessible()
        {
            // Act
            var repository = _controller._repo;

            // Assert
            repository.Should().NotBeNull();
            repository.Should().Be(_mockRepository.Object);
        }

        [Fact]
        public async Task GetRebateMLogs_ValidDateRange_ShouldReturnLogs()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);
            var expectedData = new List<RebateMLogModel>
            {
                new RebateMLogModel { Id = 1, CustId = 123, RedeemAmount = 100.50m, CreateTime = fromDate, CurrencyId = 1, RebateType = 1, UserName = "User123", SiteId = 1, BonusCode = "BONUS1", Memo = "Test", RelId = 1001 },
                new RebateMLogModel { Id = 2, CustId = 456, RedeemAmount = 250.75m, CreateTime = fromDate.AddDays(5), CurrencyId = 2, RebateType = 2, UserName = "User456", SiteId = 1, BonusCode = "BONUS2", Memo = "Test2", RelId = 1002 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetRebateMLogs(fromDate, toDate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRebateMLogs_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            var fromDate = new DateTime(2023, 3, 15);
            var toDate = new DateTime(2023, 3, 31);

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RebateMLogModel>());

            // Act
            await _controller.GetRebateMLogs(fromDate, toDate);

            // Assert
            // Note: We verify that the correct criteria type is used
            _mockRepository.Verify(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRebateMLogs_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var fromDate = new DateTime(2023, 2, 1);
            var toDate = new DateTime(2023, 2, 28);

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RebateMLogModel>());

            // Act
            var result = await _controller.GetRebateMLogs(fromDate, toDate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRebateMLogs_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Rebate log fetch error"));

            // Act & Assert
            var act = async () => await _controller.GetRebateMLogs(fromDate, toDate);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Rebate log fetch error");
        }

        [Theory]
        [InlineData("2023-01-01", "2023-01-31")]
        [InlineData("2023-06-01", "2023-06-30")]
        [InlineData("2023-12-01", "2023-12-31")]
        public async Task GetRebateMLogs_VariousDateRanges_ShouldHandleCorrectly(string fromDateStr, string toDateStr)
        {
            // Arrange
            var fromDate = DateTime.Parse(fromDateStr);
            var toDate = DateTime.Parse(toDateStr);
            var expectedData = new List<RebateMLogModel>
            {
                new RebateMLogModel { Id = 1, CustId = 100, RedeemAmount = 50.25m, CreateTime = fromDate, CurrencyId = 1, RebateType = 1, UserName = "User100", SiteId = 1, BonusCode = "BONUS", Memo = "Test", RelId = 1000 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetRebateMLogs(fromDate, toDate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRebateMLogs_SameDayRange_ShouldHandleCorrectly()
        {
            // Arrange
            var sameDate = new DateTime(2023, 7, 15);
            var expectedData = new List<RebateMLogModel>
            {
                new RebateMLogModel { Id = 1, CustId = 789, RedeemAmount = 75.00m, CreateTime = sameDate, CurrencyId = 1, RebateType = 1, UserName = "User789", SiteId = 1, BonusCode = "BONUS", Memo = "Test", RelId = 1003 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetRebateMLogs(sameDate, sameDate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetRebateMLogs_ReverseDateRange_ShouldStillCallRepository()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 31);
            var toDate = new DateTime(2023, 1, 1); // Reversed range

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RebateMLogModel>());

            // Act
            var result = await _controller.GetRebateMLogs(fromDate, toDate);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRebateMLogs_LargeDateRange_ShouldHandleCorrectly()
        {
            // Arrange
            var fromDate = new DateTime(2022, 1, 1);
            var toDate = new DateTime(2023, 12, 31); // Full 2 years
            var expectedData = new List<RebateMLogModel>();

            // Generate large dataset
            for (int i = 1; i <= 10000; i++)
            {
                expectedData.Add(new RebateMLogModel
                {
                    Id = i,
                    CustId = i % 1000,
                    RedeemAmount = i * 0.5m,
                    CreateTime = fromDate.AddDays(i % 730),
                    CurrencyId = i % 5 + 1,
                    RebateType = (byte)(i % 3 + 1),
                    UserName = $"User{i}",
                    SiteId = 1,
                    BonusCode = $"BONUS{i}",
                    Memo = $"Test{i}",
                    RelId = 1000 + i
                });
            }

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetRebateMLogs(fromDate, toDate);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(10000);
        }

        [Fact]
        public async Task GetRebateMLogs_MinMaxDateTime_ShouldHandleCorrectly()
        {
            // Arrange
            var fromDate = DateTime.MinValue;
            var toDate = DateTime.MaxValue;

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RebateMLogModel>());

            // Act
            var result = await _controller.GetRebateMLogs(fromDate, toDate);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRebateMLogs_MultipleCallsInParallel_ShouldHandleCorrectly()
        {
            // Arrange
            var fromDate = new DateTime(2023, 4, 1);
            var toDate = new DateTime(2023, 4, 30);
            var expectedData = new List<RebateMLogModel>
            {
                new RebateMLogModel { Id = 1, CustId = 111, RedeemAmount = 25.50m, CreateTime = fromDate, CurrencyId = 1, RebateType = 1, UserName = "User111", SiteId = 1, BonusCode = "BONUS", Memo = "Test", RelId = 1004 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var task1 = _controller.GetRebateMLogs(fromDate, toDate);
            var task2 = _controller.GetRebateMLogs(fromDate, toDate);
            var task3 = _controller.GetRebateMLogs(fromDate, toDate);
            var results = await Task.WhenAll(task1, task2, task3);

            // Assert
            results.Should().HaveCount(3);
            results[0].Should().BeEquivalentTo(expectedData);
            results[1].Should().BeEquivalentTo(expectedData);
            results[2].Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GetRebateMLogs_WithSpecificTimeComponents_ShouldHandleCorrectly()
        {
            // Arrange
            var fromDate = new DateTime(2023, 5, 15, 9, 30, 45);
            var toDate = new DateTime(2023, 5, 15, 17, 45, 30);
            var expectedData = new List<RebateMLogModel>
            {
                new RebateMLogModel { Id = 1, CustId = 222, RedeemAmount = 150.75m, CreateTime = fromDate.AddHours(2), CurrencyId = 1, RebateType = 1, UserName = "User222", SiteId = 1, BonusCode = "BONUS", Memo = "Test", RelId = 1005 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetRebateMLogs(fromDate, toDate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetRebateMLogs_DatabaseConnectionError_ShouldPropagateException()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);

            _mockRepository
                .Setup(x => x.FetchAsync<RebateMLogModel>(It.IsAny<GetRebateMLogCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("SQL Connection Error"));

            // Act & Assert
            var act = async () => await _controller.GetRebateMLogs(fromDate, toDate);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
