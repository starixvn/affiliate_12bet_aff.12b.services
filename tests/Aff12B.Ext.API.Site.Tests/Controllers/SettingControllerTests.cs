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
    /// Unit tests for SettingController class
    /// Tests server info and game type endpoints
    /// Target: 100% code coverage
    /// </summary>
    public class SettingControllerTests
    {
        private readonly Mock<IDynamicRepository> _mockRepository;
        private readonly SettingController _controller;

        public SettingControllerTests()
        {
            _mockRepository = new Mock<IDynamicRepository>();
            _controller = new SettingController(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Act
            var controller = new SettingController(_mockRepository.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeAssignableTo<ControllerBase>();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => new SettingController(null!);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task GetServerInfo_ShouldReturnServerInfo()
        {
            // Arrange
            var expectedData = new List<ServerInfoModel>
            {
                new ServerInfoModel { SiteId = 1, SiteName = "Server1", LangList = "en", CurrencyList = "USD", DefLang = "en", DefCurrency = 1 },
                new ServerInfoModel { SiteId = 2, SiteName = "Server2", LangList = "zh", CurrencyList = "CNY", DefLang = "zh", DefCurrency = 2 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetServerInfo();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetServerInfo_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ServerInfoModel>());

            // Act
            var result = await _controller.GetServerInfo();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetServerInfo_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var act = async () => await _controller.GetServerInfo();
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database error");
        }

        [Fact]
        public async Task GetTpGameType_ShouldReturnGameTypes()
        {
            // Arrange
            var expectedData = new List<TpGameTypeModel>
            {
                new TpGameTypeModel { Type = 1, TypeName = "Slots" },
                new TpGameTypeModel { Type = 2, TypeName = "Poker" },
                new TpGameTypeModel { Type = 3, TypeName = "Blackjack" }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTpGameType();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTpGameType_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpGameTypeModel>());

            // Act
            var result = await _controller.GetTpGameType();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTpGameType_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Game type fetch error"));

            // Act & Assert
            var act = async () => await _controller.GetTpGameType();
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Game type fetch error");
        }

        [Fact]
        public async Task GetServerInfo_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ServerInfoModel>());

            // Act
            await _controller.GetServerInfo();

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTpGameType_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpGameTypeModel>());

            // Act
            await _controller.GetTpGameType();

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetServerInfo_MultipleCallsInParallel_ShouldHandleCorrectly()
        {
            // Arrange
            var expectedData = new List<ServerInfoModel>
            {
                new ServerInfoModel { SiteId = 1, SiteName = "Server1", LangList = "en", CurrencyList = "USD", DefLang = "en", DefCurrency = 1 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var task1 = _controller.GetServerInfo();
            var task2 = _controller.GetServerInfo();
            var results = await Task.WhenAll(task1, task2);

            // Assert
            results.Should().HaveCount(2);
            results[0].Should().BeEquivalentTo(expectedData);
            results[1].Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetTpGameType_MultipleCallsInParallel_ShouldHandleCorrectly()
        {
            // Arrange
            var expectedData = new List<TpGameTypeModel>
            {
                new TpGameTypeModel { Type = 1, TypeName = "Slots" }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var task1 = _controller.GetTpGameType();
            var task2 = _controller.GetTpGameType();
            var results = await Task.WhenAll(task1, task2);

            // Assert
            results.Should().HaveCount(2);
            results[0].Should().BeEquivalentTo(expectedData);
            results[1].Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetServerInfo_WithLargeDataSet_ShouldHandleCorrectly()
        {
            // Arrange
            var expectedData = new List<ServerInfoModel>();
            for (int i = 1; i <= 1000; i++)
            {
                expectedData.Add(new ServerInfoModel { SiteId = i, SiteName = $"Server{i}", LangList = "en", CurrencyList = "USD", DefLang = "en", DefCurrency = i % 10 });
            }

            _mockRepository
                .Setup(x => x.FetchAsync<ServerInfoModel>(It.IsAny<GetServerInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetServerInfo();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1000);
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetTpGameType_WithLargeDataSet_ShouldHandleCorrectly()
        {
            // Arrange
            var expectedData = new List<TpGameTypeModel>();
            for (int i = 1; i <= 500; i++)
            {
                expectedData.Add(new TpGameTypeModel { Type = i, TypeName = $"GameType{i}" });
            }

            _mockRepository
                .Setup(x => x.FetchAsync<TpGameTypeModel>(It.IsAny<GetTpGameTypeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTpGameType();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(500);
            result.Should().BeEquivalentTo(expectedData);
        }
    }
}
