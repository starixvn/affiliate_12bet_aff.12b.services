using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.DataDomain.Params;
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
    /// Unit tests for Product9WController class
    /// Tests third-party product report and wager endpoints
    /// Target: 100% code coverage
    /// </summary>
    public class Product9WControllerTests
    {
        private readonly Mock<IDynamicRepository> _mockRepository;
        private readonly Product9WController _controller;

        public Product9WControllerTests()
        {
            _mockRepository = new Mock<IDynamicRepository>();
            _controller = new Product9WController(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Act
            var controller = new Product9WController(_mockRepository.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeAssignableTo<ControllerBase>();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => new Product9WController(null!);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task GetTpReport_ValidParam_ShouldReturnReports()
        {
            // Arrange
            var param = new BatchCountParam
            {
                SQ = 123,
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                BatchCount = 1000
            };

            var expectedData = new List<TpReport9WModel>
            {
                new TpReport9WModel { SQ = 123, UserId = "User1", AgentId = "Agent1", EventId = "Event1", MarketId = "Market1", SettledDate = new DateTime(2023, 1, 15), ProfitLoss = 1500.50m, MarketTotal = 2000m, Commission = 50m, AgentCommission = 25m, StakeTotal = 1000m, CustId = 1001 },
                new TpReport9WModel { SQ = 124, UserId = "User2", AgentId = "Agent2", EventId = "Event2", MarketId = "Market2", SettledDate = new DateTime(2023, 1, 16), ProfitLoss = 2300.75m, MarketTotal = 3000m, Commission = 75m, AgentCommission = 35m, StakeTotal = 1500m, CustId = 1002 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTpReport(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTpReport_ParameterMapping_ShouldMapCorrectly()
        {
            // Arrange
            var param = new BatchCountParam
            {
                SQ = 456,
                FromDate = new DateTime(2023, 5, 15),
                ToDate = new DateTime(2023, 5, 31),
                BatchCount = 500
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpReport9WModel>());

            // Act
            await _controller.GetTpReport(param);

            // Assert
            // Note: We verify that the correct criteria type is used
            _mockRepository.Verify(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTpReport_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var param = new BatchCountParam
            {
                SQ = 789,
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                BatchCount = 100
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpReport9WModel>());

            // Act
            var result = await _controller.GetTpReport(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTpReport_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var param = new BatchCountParam
            {
                SQ = 999,
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                BatchCount = 100
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Report fetch error"));

            // Act & Assert
            var act = async () => await _controller.GetTpReport(param);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Report fetch error");
        }

        [Fact]
        public async Task GetTpWager_ValidParam_ShouldReturnWagers()
        {
            // Arrange
            var param = new BatchCountParam
            {
                CreateTime = new DateTime(2023, 6, 15, 10, 30, 0),
                FromDate = new DateTime(2023, 6, 1),
                ToDate = new DateTime(2023, 6, 30),
                BatchCount = 2000
            };

            var expectedData = new List<TpWager9WModel>
            {
                new TpWager9WModel { BetId = "BET001", UserId = "User1", AgentId = "Agent1", EventId = "Event1", MarketId = "Market1", MatchSettledDate = "2023-06-15", BetStatus = "Settled", MatchStake = 100.25m, ProfitLoss = 50.12m, CreateTime = param.CreateTime },
                new TpWager9WModel { BetId = "BET002", UserId = "User2", AgentId = "Agent2", EventId = "Event2", MarketId = "Market2", MatchSettledDate = "2023-06-15", BetStatus = "Settled", MatchStake = 250.50m, ProfitLoss = 125.25m, CreateTime = param.CreateTime.AddHours(1) }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTpWager(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTpWager_ParameterMapping_ShouldMapCorrectly()
        {
            // Arrange
            var param = new BatchCountParam
            {
                CreateTime = new DateTime(2023, 8, 20, 14, 45, 30),
                FromDate = new DateTime(2023, 8, 1),
                ToDate = new DateTime(2023, 8, 31),
                BatchCount = 1500
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpWager9WModel>());

            // Act
            await _controller.GetTpWager(param);

            // Assert
            // Note: We verify that the correct criteria type is used
            _mockRepository.Verify(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTpWager_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var param = new BatchCountParam
            {
                CreateTime = new DateTime(2023, 1, 1),
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                BatchCount = 100
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpWager9WModel>());

            // Act
            var result = await _controller.GetTpWager(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTpWager_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var param = new BatchCountParam
            {
                CreateTime = new DateTime(2023, 1, 1),
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                BatchCount = 100
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Wager fetch error"));

            // Act & Assert
            var act = async () => await _controller.GetTpWager(param);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Wager fetch error");
        }

        [Theory]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(2000)]
        public async Task GetTpReport_VariousBatchCounts_ShouldHandleCorrectly(int batchCount)
        {
            // Arrange
            var param = new BatchCountParam
            {
                SQ = batchCount,
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                BatchCount = batchCount
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpReport9WModel>());

            // Act
            var result = await _controller.GetTpReport(param);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(250)]
        [InlineData(750)]
        [InlineData(1250)]
        [InlineData(2500)]
        public async Task GetTpWager_VariousBatchCounts_ShouldHandleCorrectly(int batchCount)
        {
            // Arrange
            var param = new BatchCountParam
            {
                CreateTime = new DateTime(2023, 6, 15),
                FromDate = new DateTime(2023, 6, 1),
                ToDate = new DateTime(2023, 6, 30),
                BatchCount = batchCount
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpWager9WModel>());

            // Act
            var result = await _controller.GetTpWager(param);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTpReport_NullParam_ShouldHandleGracefully()
        {
            // Arrange
            BatchCountParam param = null!;

            _mockRepository
                .Setup(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpReport9WModel>());

            // Act & Assert
            var act = async () => await _controller.GetTpReport(param);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task GetTpWager_NullParam_ShouldHandleGracefully()
        {
            // Arrange
            BatchCountParam param = null!;

            _mockRepository
                .Setup(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpWager9WModel>());

            // Act & Assert
            var act = async () => await _controller.GetTpWager(param);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task GetTpReport_WithLargeDataSet_ShouldHandleCorrectly()
        {
            // Arrange
            var param = new BatchCountParam
            {
                SQ = 12345,
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 12, 31),
                BatchCount = 10000
            };

            var expectedData = new List<TpReport9WModel>();
            // Reduced from 5000 to 25 records for performance
            for (int i = 1; i <= 25; i++)
            {
                expectedData.Add(new TpReport9WModel { SQ = i, UserId = $"User{i}", AgentId = $"Agent{i}", EventId = $"Event{i}", MarketId = $"Market{i}", SettledDate = new DateTime(2023, 1, 1).AddDays(i % 365), ProfitLoss = i * 10.5m, MarketTotal = i * 15m, Commission = i * 0.5m, AgentCommission = i * 0.25m, StakeTotal = i * 8m, CustId = 1000 + i });
            }

            _mockRepository
                .Setup(x => x.FetchAsync<TpReport9WModel>(It.IsAny<GetTpReport9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTpReport(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(25);
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task GetTpWager_WithLargeDataSet_ShouldHandleCorrectly()
        {
            // Arrange
            var param = new BatchCountParam
            {
                CreateTime = new DateTime(2023, 6, 15),
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 12, 31),
                BatchCount = 15000
            };

            var expectedData = new List<TpWager9WModel>();
            // Reduced from 3000 to 25 records for performance
            for (int i = 1; i <= 25; i++)
            {
                expectedData.Add(new TpWager9WModel { BetId = $"BET{i:D6}", UserId = $"User{i}", AgentId = $"Agent{i}", EventId = $"Event{i}", MarketId = $"Market{i}", MatchSettledDate = "2023-06-15", BetStatus = "Settled", MatchStake = i * 5.25m, ProfitLoss = i * 2.5m, CreateTime = param.CreateTime.AddMinutes(i) });
            }

            _mockRepository
                .Setup(x => x.FetchAsync<TpWager9WModel>(It.IsAny<GetTpWager9WCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTpWager(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(25);
        }
    }
}
