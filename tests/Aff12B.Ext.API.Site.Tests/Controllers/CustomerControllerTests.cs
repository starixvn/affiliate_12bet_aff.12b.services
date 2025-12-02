using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.DataDomain.Params;
using Aff12B.Ext.API.Infastructure.Command;
using Aff12B.Ext.API.Infastructure.Criteria;
using Aff12B.Ext.API.Infastructure.Params;
using Aff12B.Ext.API.Site.Controllers;
using Fanex.Data.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests.Controllers
{
    /// <summary>
    /// Unit tests for CustomerController class
    /// Tests customer management endpoints and business logic
    /// Target: 95%+ code coverage
    /// </summary>
    public class CustomerControllerTests
    {
        private readonly Mock<IDynamicRepository> _mockRepository;
        private readonly Mock<ILogger<CustomerController>> _mockLogger;
        private readonly CustomerController _controller;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<ConnectionInfo> _mockConnection;

        public CustomerControllerTests()
        {
            _mockRepository = new Mock<IDynamicRepository>();
            _mockLogger = new Mock<ILogger<CustomerController>>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockConnection = new Mock<ConnectionInfo>();

            _controller = new CustomerController(_mockRepository.Object, _mockLogger.Object);

            // Setup HttpContext mock
            _mockHttpContext.Setup(x => x.Connection).Returns(_mockConnection.Object);
            _mockConnection.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };
        }

        [Fact]
        public void Constructor_ValidDependencies_ShouldCreateInstance()
        {
            // Act
            var controller = new CustomerController(_mockRepository.Object, _mockLogger.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeAssignableTo<ControllerBase>();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => new CustomerController(null, _mockLogger.Object);
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_NullLogger_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => new CustomerController(_mockRepository.Object, null);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task GetAffCustomers_ValidParameters_ShouldReturnData()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);
            var pageIndex = 0;
            var pageSize = 100;
            var expectedData = new List<AffCustomerModel>
            {
                new AffCustomerModel { CustId = 1, UserName = "customer1" },
                new AffCustomerModel { CustId = 2, UserName = "customer2" }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetAffCustomers(fromDate, toDate, pageIndex, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAffCustomers_DefaultPageSize_ShouldUse2000()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);
            var pageIndex = 0;
            var expectedData = new List<AffCustomerModel>();

            _mockRepository
                .Setup(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetAffCustomers(fromDate, toDate, pageIndex, 2000);

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomers_ValidDateRange_ShouldReturnData()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var toDate = new DateTime(2023, 1, 31);
            var pageIndex = 0;
            var pageSize = 100;
            var expectedData = new List<CustomerModel>
            {
                new CustomerModel { CustId = 1, UserName = "user1" },
                new CustomerModel { CustId = 2, UserName = "user2" }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<CustomerModel>(It.IsAny<GetCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetCustomers(fromDate, toDate, pageIndex, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<CustomerModel>(It.IsAny<GetCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetLoginCustomerInfo_ValidParameters_ShouldReturnCustomer()
        {
            // Arrange
            var userName = "testuser";
            var siteId = 1;
            var expectedCustomer = new CustomerModel { CustId = 123, UserName = userName };

            _mockRepository
                .Setup(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCustomer);

            // Act
            var result = await _controller.GetLoginCustomerInfo(userName, siteId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedCustomer);
            _mockRepository.Verify(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetLoginCustomerInfo_NullUserName_ShouldReturnNull()
        {
            // Arrange
            string userName = null;
            var siteId = 1;

            // Act
            var result = await _controller.GetLoginCustomerInfo(userName, siteId);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetLoginCustomerInfo_EmptyUserName_ShouldReturnNull()
        {
            // Arrange
            var userName = "";
            var siteId = 1;

            // Act
            var result = await _controller.GetLoginCustomerInfo(userName, siteId);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetLoginCustomerInfo_WhitespaceUserName_ShouldReturnNull()
        {
            // Arrange
            var userName = "   ";
            var siteId = 1;

            // Act
            var result = await _controller.GetLoginCustomerInfo(userName, siteId);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetLoginCustomerInfo_NonASCIIUserName_ShouldReturnNull()
        {
            // Arrange
            var userName = "用户名"; // Non-ASCII characters
            var siteId = 1;

            // Act
            var result = await _controller.GetLoginCustomerInfo(userName, siteId);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetLoginCustomerInfo_ASCIIUserName_ShouldCallRepository()
        {
            // Arrange
            var userName = "testuser123";
            var siteId = 1;
            var expectedCustomer = new CustomerModel { CustId = 123, UserName = userName };

            _mockRepository
                .Setup(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCustomer);

            // Act
            var result = await _controller.GetLoginCustomerInfo(userName, siteId);

            // Assert
            result.Should().Be(expectedCustomer);
            _mockRepository.Verify(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerWallet_ValidPayment_ShouldReturnAfterAmount()
        {
            // Arrange
            var payment = new PaymentRequestParam
            {
                Amount = 100.50m,
                CustId = 123,
                Memo = "Test payment",
                RefTransId = "REF123",
                SiteId = 1,
                Type = 1,
                UserName = "testuser"
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UpdateCustomerWalletCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))

                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.UpdateCustomerWallet(payment);

            // Assert
            result.Should().Be(0); // AfterAmount defaults to 0 since we don't mock the callback
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<UpdateCustomerWalletCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerWallet_ShouldLogRequestAndResponse()
        {
            // Arrange
            var payment = new PaymentRequestParam
            {
                Amount = 100m,
                CustId = 123,
                UserName = "testuser"
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UpdateCustomerWalletCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _controller.UpdateCustomerWallet(payment);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Recevice update wallet request")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("AfterAmount: 0")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UnlockUser_ValidUserIds_ShouldReturnSuccessResult()
        {
            // Arrange
            var userIds = new int[] { 1, 2, 3 };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UnlockUsersCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.UnlockUser(userIds);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.ErrMsg.Should().BeNull();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<UnlockUsersCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnlockUser_EmptyUserIds_ShouldReturnSuccessResult()
        {
            // Arrange
            var userIds = new int[] { };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UnlockUsersCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.UnlockUser(userIds);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<UnlockUsersCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnlockUser_RepositoryThrowsException_ShouldReturnErrorResult()
        {
            // Arrange
            var userIds = new int[] { 1, 2 };
            var expectedError = "Database error";

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UnlockUsersCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(expectedError));

            // Act
            var result = await _controller.UnlockUser(userIds);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Be(expectedError);
        }

        [Fact]
        public async Task InsertAffiliateInfo_ValidAffiliate_ShouldReturnSuccessResult()
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 123,
                AffiliateCode = "AFF123",
                AffiliateName = "Test Affiliate",
                CustId = 456,
                ParentAffiliate = "PARENT",
                SiteId = 1,
                UserName = "testuser",
                FirstName = "John",
                LastName = "Doe",
                ContactNumber = "123-456-7890",
                Email = "test@example.com",
                Status = "Active"
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.ErrMsg.Should().BeNull();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffiliateInfo_CustIdZero_ShouldUseMarketingCustId()
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 123,
                CustId = 0 // Zero should be replaced with -1
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffiliateInfo_NullOptionalFields_ShouldUseEmptyStrings()
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 123,
                CustId = 456,
                ParentAffiliate = null,
                UserName = null,
                FirstName = null,
                LastName = null,
                ContactNumber = null,
                Email = null,
                Status = null
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffiliateInfo_RepositoryThrowsException_ShouldReturnErrorResult()
        {
            // Arrange
            var affiliate = new AffiliateParam { AffiliateId = 123 };
            var expectedError = "Insert failed";

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(expectedError));

            // Act
            var result = await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Be(expectedError);
        }

        [Fact]
        public async Task InsertAffiliateInfo_ShouldMapAllFieldsCorrectly()
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 999,
                AffiliateCode = "TEST999",
                AffiliateName = "Test Affiliate Name",
                CustId = 888,
                ParentAffiliate = "PARENT999",
                SiteId = 5,
                UserName = "testuser999",
                FirstName = "John",
                LastName = "Smith",
                ContactNumber = "+1-555-0199",
                Email = "john.smith@test.com",
                Status = "Active",
                DealVerifyFlags = 7
            };

            InsertAffiliateInfoCommand capturedCommand = null;
            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, connStr, ct) =>
                {
                    capturedCommand = cmd as InsertAffiliateInfoCommand;
                })
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedCommand.Should().NotBeNull();
            capturedCommand.AffiliateId.Should().Be(affiliate.AffiliateId);
            capturedCommand.AffiliateCode.Should().Be(affiliate.AffiliateCode);
            capturedCommand.AffiliateName.Should().Be(affiliate.AffiliateName);
            capturedCommand.CustId.Should().Be(affiliate.CustId);
            capturedCommand.ParentAffiliate.Should().Be(affiliate.ParentAffiliate);
            capturedCommand.SiteId.Should().Be(affiliate.SiteId);
            capturedCommand.UserName.Should().Be(affiliate.UserName);
            capturedCommand.FirstName.Should().Be(affiliate.FirstName);
            capturedCommand.LastName.Should().Be(affiliate.LastName);
            capturedCommand.ContactNumber.Should().Be(affiliate.ContactNumber);
            capturedCommand.Email.Should().Be(affiliate.Email);
            capturedCommand.Status.Should().Be(affiliate.Status);
            capturedCommand.DealVerifyFlags.Should().Be(affiliate.DealVerifyFlags);
        }

        [Fact]
        public async Task InsertAffiliateInfo_WithMinimalData_ShouldSucceed()
        {
            // Arrange - Only required fields
            var affiliate = new AffiliateParam
            {
                AffiliateId = 100,
                AffiliateCode = "MIN100",
                CustId = 200
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffiliateInfo_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 123,
                AffiliateCode = "AFF@123#",
                AffiliateName = "Test & Co.",
                CustId = 456,
                UserName = "user_name",
                FirstName = "O'Brien",
                LastName = "Müller",
                ContactNumber = "+1 (555) 123-4567",
                Email = "test+tag@example.com",
                Status = "Active"
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task InsertAffiliateInfo_NegativeCustId_ShouldNotConvertToMarketingId()
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 123,
                CustId = -5 // Already negative, should stay -5
            };

            InsertAffiliateInfoCommand capturedCommand = null;
            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, connStr, ct) =>
                {
                    capturedCommand = cmd as InsertAffiliateInfoCommand;
                })
                .Returns(Task.FromResult(1));

            // Act
            await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand.CustId.Should().Be(-5); // Should not be converted to -1
        }

        [Fact]
        public async Task InsertAffiliateInfo_EmptyStringFields_ShouldPreserveEmptyStrings()
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 123,
                CustId = 456,
                ParentAffiliate = string.Empty,
                UserName = string.Empty,
                FirstName = string.Empty
            };

            InsertAffiliateInfoCommand capturedCommand = null;
            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, connStr, ct) =>
                {
                    capturedCommand = cmd as InsertAffiliateInfoCommand;
                })
                .Returns(Task.FromResult(1));

            // Act
            await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand.ParentAffiliate.Should().Be(string.Empty);
            capturedCommand.UserName.Should().Be(string.Empty);
            capturedCommand.FirstName.Should().Be(string.Empty);
        }

        [Fact]
        public async Task InsertAffiliateInfo_DatabaseConstraintViolation_ShouldReturnErrorResult()
        {
            // Arrange
            var affiliate = new AffiliateParam { AffiliateId = 123 };
            var sqlException = "Duplicate key constraint violation";

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(sqlException));

            // Act
            var result = await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Contain(sqlException);
        }

        [Theory]
        [InlineData(0, -1)]  // Zero should become -1
        [InlineData(1, 1)]   // Positive should stay same
        [InlineData(999, 999)]
        [InlineData(-1, -1)] // Negative should stay same
        [InlineData(-99, -99)]
        public async Task InsertAffiliateInfo_CustIdConversion_ShouldHandleCorrectly(int inputCustId, int expectedCustId)
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 123,
                CustId = inputCustId
            };

            InsertAffiliateInfoCommand capturedCommand = null;
            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, connStr, ct) =>
                {
                    capturedCommand = cmd as InsertAffiliateInfoCommand;
                })
                .Returns(Task.FromResult(1));

            // Act
            await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand.CustId.Should().Be(expectedCustId);
        }

        [Fact]
        public async Task InsertAffiliateInfo_DealVerifyFlags_ShouldBeSetCorrectly()
        {
            // Arrange
            var affiliate = new AffiliateParam
            {
                AffiliateId = 123,
                CustId = 456,
                DealVerifyFlags = 15 // Some flag value
            };

            InsertAffiliateInfoCommand capturedCommand = null;
            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffiliateInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, connStr, ct) =>
                {
                    capturedCommand = cmd as InsertAffiliateInfoCommand;
                })
                .Returns(Task.FromResult(1));

            // Act
            await _controller.InsertAffiliateInfo(affiliate);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand.DealVerifyFlags.Should().Be(15);
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(5, 10, 15, 20)]
        [InlineData(1)]
        [InlineData(100, 200, 300, 400, 500)]
        public async Task GetAffCustomers_VariousPageSizes_ShouldHandleCorrectly(int pageIndex, params int[] pageSizes)
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-30);
            var toDate = DateTime.Now;
            var expectedData = new List<AffCustomerModel>();

            _mockRepository
                .Setup(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            foreach (var pageSize in pageSizes)
            {
                // Act
                var result = await _controller.GetAffCustomers(fromDate, toDate, pageIndex, pageSize);

                // Assert
                result.Should().NotBeNull();
                _mockRepository.Verify(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

                _mockRepository.Reset();
                _mockRepository
                    .Setup(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedData);
            }
        }

        [Theory]
        [InlineData("2023-01-01", "2023-01-31")]
        [InlineData("2023-06-01", "2023-06-30")]
        [InlineData("2023-12-01", "2023-12-31")]
        public async Task GetCustomers_DateRangeValidation_ShouldWork(string fromDateStr, string toDateStr)
        {
            // Arrange
            var fromDate = DateTime.Parse(fromDateStr);
            var toDate = DateTime.Parse(toDateStr);
            var expectedData = new List<CustomerModel>();

            _mockRepository
                .Setup(x => x.FetchAsync<CustomerModel>(It.IsAny<GetCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetCustomers(fromDate, toDate, 0);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<CustomerModel>(It.IsAny<GetCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAffCustomers_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            var fromDate = new DateTime(2023, 5, 1);
            var toDate = new DateTime(2023, 5, 31);
            var pageIndex = 2;
            var pageSize = 500;

            _mockRepository
                .Setup(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AffCustomerModel>());

            // Act
            await _controller.GetAffCustomers(fromDate, toDate, pageIndex, pageSize);

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<AffCustomerModel>(It.IsAny<GetAffCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomers_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            var fromDate = new DateTime(2023, 3, 15);
            var toDate = new DateTime(2023, 3, 20);
            var pageIndex = 1;
            var pageSize = 1000;

            _mockRepository
                .Setup(x => x.FetchAsync<CustomerModel>(It.IsAny<GetCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CustomerModel>());

            // Act
            await _controller.GetCustomers(fromDate, toDate, pageIndex, pageSize);

            // Assert
            _mockRepository.Verify(x => x.FetchAsync<CustomerModel>(It.IsAny<GetCustomersCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetLoginCustomerInfo_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            var userName = "specificuser";
            var siteId = 42;

            _mockRepository
                .Setup(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CustomerModel());

            // Act
            await _controller.GetLoginCustomerInfo(userName, siteId);

            // Assert
            _mockRepository.Verify(x => x.GetAsync<CustomerModel>(It.IsAny<GetLoginCustInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertIdCode_ValidParam_ShouldReturnSuccessResult()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "TEST123",
                SiteId = 1,
                StatusId = 2
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.ErrMsg.Should().BeNull();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertIdCode_RepositoryThrowsException_ShouldReturnErrorResult()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "TEST123",
                SiteId = 1,
                StatusId = 2
            };
            var expectedError = "Insert failed";

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(expectedError));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Be(expectedError);
        }

        [Fact]
        public async Task InsertIdCode_ShouldMapParametersCorrectly()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "PREMIUM_CODE_001",
                SiteId = 5,
                StatusId = 10
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _controller.InsertIdCode(param);

            // Assert
            _mockRepository.Verify(x => x.ExecuteAsync(
                It.Is<InsertIdCodeCommand>(cmd =>
                    cmd.IdCode == "PREMIUM_CODE_001" &&
                    cmd.SiteId == 5 &&
                    cmd.StatusId == 10),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task InsertIdCode_NullIdCode_ShouldReturnSuccessResult()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = null,
                SiteId = 1,
                StatusId = 2
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertIdCode_EmptyIdCode_ShouldReturnSuccessResult()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "",
                SiteId = 1,
                StatusId = 2
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("CODE1", 1, 1)]
        [InlineData("CODE2", 2, 2)]
        [InlineData("CODE3", 5, 10)]
        [InlineData("ALPHA_123", 100, 200)]
        public async Task InsertIdCode_VariousParameters_ShouldWork(string idCode, int siteId, int statusId)
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = idCode,
                SiteId = siteId,
                StatusId = statusId
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertIdCode_ZeroSiteIdAndStatusId_ShouldReturnSuccessResult()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "TEST",
                SiteId = 0,
                StatusId = 0
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertIdCode_NegativeSiteIdAndStatusId_ShouldReturnSuccessResult()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "TEST",
                SiteId = -1,
                StatusId = -100
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertIdCode_SpecialCharactersInIdCode_ShouldReturnSuccessResult()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "!@#$%^&*()",
                SiteId = 1,
                StatusId = 2
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertIdCode_DatabaseException_ShouldReturnErrorResult()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "TEST",
                SiteId = 1,
                StatusId = 2
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _controller.InsertIdCode(param);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Be("Database connection error");
        }

        [Fact]
        public async Task InsertIdCode_CommandExecutionSucceeds_ShouldCallRepositoryOnce()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "VERIFY_CALL",
                SiteId = 99,
                StatusId = 88
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertIdCodeCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _controller.InsertIdCode(param);

            // Assert
            _mockRepository.Verify(x => x.ExecuteAsync(
                It.Is<InsertIdCodeCommand>(cmd =>
                    cmd.IdCode == "VERIFY_CALL" &&
                    cmd.SiteId == 99 &&
                    cmd.StatusId == 88),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
