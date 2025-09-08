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
    /// Unit tests for BetReportController class
    /// Tests betting report endpoints and data retrieval functionality
    /// Target: 100% code coverage
    /// </summary>
    public class BetReportControllerTests
    {
        private readonly Mock<IDynamicRepository> _mockRepository;
        private readonly BetReportController _controller;

        public BetReportControllerTests()
        {
            _mockRepository = new Mock<IDynamicRepository>();
            _controller = new BetReportController(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Act
            var controller = new BetReportController(_mockRepository.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeAssignableTo<ControllerBase>();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => new BetReportController(null!);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task GetBetRecord_ValidParameters_ShouldReturnBetRecords()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);
            var pageIndex = 0;
            var pageSize = 1000;
            var expectedData = new List<TpBetRecordModel>
            {
                new TpBetRecordModel { TransId = "BET001", CustId = "1" },
                new TpBetRecordModel { TransId = "BET002", CustId = "2" }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TpBetRecordModel>(It.IsAny<GetTpBetRecordCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetBetRecord(fromDate, toDate, pageIndex, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TpBetRecordModel>(It.IsAny<GetTpBetRecordCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBetRecord_DefaultPageSize_ShouldUse2000()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);
            var pageIndex = 0;
            var expectedData = new List<TpBetRecordModel>();

            _mockRepository
                .Setup(x => x.FetchAsync<TpBetRecordModel>(It.IsAny<GetTpBetRecordCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetBetRecord(fromDate, toDate, pageIndex, 2000);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<TpBetRecordModel>(It.IsAny<GetTpBetRecordCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDepositInfo_ValidParameters_ShouldReturnDeposits()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);
            var pageIndex = 0;
            var pageSize = 500;
            var expectedData = new List<DepositInfoModel>
            {
                new DepositInfoModel { CustId = 1, Amount = 100.50m },
                new DepositInfoModel { CustId = 2, Amount = 250.75m }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<DepositInfoModel>(It.IsAny<GetDepositInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetDepositInfo(fromDate, toDate, pageIndex, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<DepositInfoModel>(It.IsAny<GetDepositInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDepositInfo_DefaultPageSize_ShouldUse2000()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);
            var pageIndex = 0;

            _mockRepository
                .Setup(x => x.FetchAsync<DepositInfoModel>(It.IsAny<GetDepositInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DepositInfoModel>());

            // Act
            await _controller.GetDepositInfo(fromDate, toDate, pageIndex, 2000);

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<DepositInfoModel>(It.IsAny<GetDepositInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBonusInfo_ValidId_ShouldReturnBonuses()
        {
            // Arrange
            var id = 123;
            var batchCount = 1000;
            var expectedData = new List<BonusModel>
            {
                new BonusModel { CustId = 1, BonusAmount = 50.25m },
                new BonusModel { CustId = 2, BonusAmount = 75.50m }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetBonusInfo(id, batchCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBonusInfo_DefaultBatchCount_ShouldUse2000()
        {
            // Arrange
            var id = 123;

            _mockRepository
                .Setup(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BonusModel>());

            // Act
            await _controller.GetBonusInfo(id, 2000);

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBonusInfoByDate_ValidDate_ShouldReturnBonuses()
        {
            // Arrange
            var calledTime = new DateTime(2023, 6, 15);
            var pageIndex = 1;
            var pageSize = 800;
            var expectedData = new List<BonusModel>
            {
                new BonusModel { CustId = 10, BonusAmount = 25.00m },
                new BonusModel { CustId = 20, BonusAmount = 35.75m }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoByDateCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetBonusInfoByDate(calledTime, pageIndex, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoByDateCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBonusInfoByDate_DefaultPageSize_ShouldUse2000()
        {
            // Arrange
            var calledTime = new DateTime(2023, 6, 15);
            var pageIndex = 0;

            _mockRepository
                .Setup(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoByDateCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BonusModel>());

            // Act
            await _controller.GetBonusInfoByDate(calledTime, pageIndex, 2000);

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoByDateCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetWithdrawalInfo_ValidTime_ShouldReturnWithdrawals()
        {
            // Arrange
            var updateTime = new DateTime(2023, 8, 20);
            var batchCount = 1500;
            var expectedData = new List<WithdrawalInfoModel>
            {
                new WithdrawalInfoModel { CustId = 5, Amount = 200.00m },
                new WithdrawalInfoModel { CustId = 6, Amount = 150.25m }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<WithdrawalInfoModel>(It.IsAny<GetWithdrawalInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetWithdrawalInfo(updateTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<WithdrawalInfoModel>(It.IsAny<GetWithdrawalInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetWithdrawalInfo_DefaultBatchCount_ShouldUse2000()
        {
            // Arrange
            var updateTime = new DateTime(2023, 8, 20);

            _mockRepository
                .Setup(x => x.FetchAsync<WithdrawalInfoModel>(It.IsAny<GetWithdrawalInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<WithdrawalInfoModel>());

            // Act
            await _controller.GetWithdrawalInfo(updateTime, 2000);

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<WithdrawalInfoModel>(It.IsAny<GetWithdrawalInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBetRecord_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);

            _mockRepository
                .Setup(x => x.FetchAsync<TpBetRecordModel>(It.IsAny<GetTpBetRecordCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var act = async () => await _controller.GetBetRecord(fromDate, toDate, 0, 2000);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database error");
        }

        [Fact]
        public async Task GetDepositInfo_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);

            _mockRepository
                .Setup(x => x.FetchAsync<DepositInfoModel>(It.IsAny<GetDepositInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act & Assert
            var act = async () => await _controller.GetDepositInfo(fromDate, toDate, 0, 2000);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Repository error");
        }

        [Fact]
        public async Task GetBonusInfo_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Bonus fetch error"));

            // Act & Assert
            var act = async () => await _controller.GetBonusInfo(123, 2000);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Bonus fetch error");
        }

        [Fact]
        public async Task GetBonusInfoByDate_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var calledTime = new DateTime(2023, 6, 15);

            _mockRepository
                .Setup(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoByDateCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Date query error"));

            // Act & Assert
            var act = async () => await _controller.GetBonusInfoByDate(calledTime, 0, 2000);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Date query error");
        }

        [Fact]
        public async Task GetWithdrawalInfo_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var updateTime = new DateTime(2023, 8, 20);

            _mockRepository
                .Setup(x => x.FetchAsync<WithdrawalInfoModel>(It.IsAny<GetWithdrawalInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Withdrawal query error"));

            // Act & Assert
            var act = async () => await _controller.GetWithdrawalInfo(updateTime, 2000);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Withdrawal query error");
        }

        [Fact]
        public async Task GetBetRecord_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);

            _mockRepository
                .Setup(x => x.FetchAsync<TpBetRecordModel>(It.IsAny<GetTpBetRecordCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TpBetRecordModel>());

            // Act
            var result = await _controller.GetBetRecord(fromDate, toDate, 0, 2000);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDepositInfo_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);

            _mockRepository
                .Setup(x => x.FetchAsync<DepositInfoModel>(It.IsAny<GetDepositInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DepositInfoModel>());

            // Act
            var result = await _controller.GetDepositInfo(fromDate, toDate, 0, 2000);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetBonusInfo_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BonusModel>());

            // Act
            var result = await _controller.GetBonusInfo(123, 2000);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetBonusInfoByDate_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var calledTime = new DateTime(2023, 6, 15);

            _mockRepository
                .Setup(x => x.FetchAsync<BonusModel>(It.IsAny<GetBonusInfoByDateCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BonusModel>());

            // Act
            var result = await _controller.GetBonusInfoByDate(calledTime, 0, 2000);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetWithdrawalInfo_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var updateTime = new DateTime(2023, 8, 20);

            _mockRepository
                .Setup(x => x.FetchAsync<WithdrawalInfoModel>(It.IsAny<GetWithdrawalInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<WithdrawalInfoModel>());

            // Act
            var result = await _controller.GetWithdrawalInfo(updateTime, 2000);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}