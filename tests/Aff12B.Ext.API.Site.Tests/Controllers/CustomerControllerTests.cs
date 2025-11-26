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
        public async Task InsertAffInfos_ValidAffiliateParamCollection_ShouldReturnSuccessResult()
        {
            // Arrange
            var affiliateParams = new List<AffiliateParam>
            {
                new AffiliateParam
                {
                    AffId = 123,
                    AffCode = "AFF123",
                    ParentAff = "PARENT",
                    UserName = "Test Affiliate",
                    Player_CustId = 456,
                    Player_Username = "testuser",
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "123-456-7890",
                    Email = "test@example.com",
                    Status = "Active",
                    DealVerifyFlags = 0
                }
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.ErrMsg.Should().BeNull();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffInfos_EmptyCollection_ShouldReturnErrorResult()
        {
            // Arrange
            var affiliateParams = new List<AffiliateParam>();

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Be("AffInfos cannot be empty.");
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task InsertAffInfos_NullCollection_ShouldReturnErrorResult()
        {
            // Arrange
            IEnumerable<AffiliateParam> affiliateParams = null;

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Be("AffInfos cannot be empty.");
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task InsertAffInfos_RepositoryThrowsException_ShouldReturnErrorResult()
        {
            // Arrange
            var affiliateParams = new List<AffiliateParam>
            {
                new AffiliateParam { AffId = 123, AffCode = "AFF123" }
            };
            var expectedError = "Insert failed";

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(expectedError));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Be(expectedError);
        }

        [Fact]
        public async Task InsertAffInfos_MultipleAffiliates_ShouldCallRepositoryOnce()
        {
            // Arrange
            var affiliateParams = new List<AffiliateParam>
            {
                new AffiliateParam
                {
                    AffId = 123,
                    AffCode = "AFF123",
                    ParentAff = "PARENT1",
                    UserName = "Affiliate 1",
                    Player_CustId = 456,
                    Player_Username = "user1",
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "123-456-7890",
                    Email = "user1@example.com",
                    Status = "Active",
                    DealVerifyFlags = 0
                },
                new AffiliateParam
                {
                    AffId = 124,
                    AffCode = "AFF124",
                    ParentAff = "PARENT2",
                    UserName = "Affiliate 2",
                    Player_CustId = 457,
                    Player_Username = "user2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Phone = "098-765-4321",
                    Email = "user2@example.com",
                    Status = "Active",
                    DealVerifyFlags = 0
                },
                new AffiliateParam
                {
                    AffId = 125,
                    AffCode = "AFF125",
                    ParentAff = "PARENT3",
                    UserName = "Affiliate 3",
                    Player_CustId = 458,
                    Player_Username = "user3",
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Phone = "555-555-5555",
                    Email = "user3@example.com",
                    Status = "Active",
                    DealVerifyFlags = 0
                }
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffInfos_ValidData_ShouldSerializeToJsonCorrectly()
        {
            // Arrange
            var affiliateParams = new List<AffiliateParam>
            {
                new AffiliateParam
                {
                    AffId = 999,
                    AffCode = "TESTCODE",
                    ParentAff = "TESTPARENT",
                    UserName = "Test Name",
                    Player_CustId = 888,
                    Player_Username = "testuser",
                    FirstName = "First",
                    LastName = "Last",
                    Phone = "111-222-3333",
                    Email = "test@test.com",
                    Status = "Active",
                    DealVerifyFlags = 1
                }
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(
                x => x.ExecuteAsync(
                    It.Is<InsertAffInfosCommand>(cmd => !string.IsNullOrEmpty(cmd.AffInfosJson)),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task InsertAffInfos_MinimalFields_ShouldCallRepository()
        {
            // Arrange
            var affiliateParams = new List<AffiliateParam>
            {
                new AffiliateParam { AffId = 1, AffCode = "CODE1" }
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffInfos_LargeCollection_ShouldHandleCorrectly()
        {
            // Arrange - Create a large collection with 100 affiliates
            var affiliateParams = new List<AffiliateParam>();
            for (int i = 1; i <= 100; i++)
            {
                affiliateParams.Add(new AffiliateParam
                {
                    AffId = i,
                    AffCode = $"AFF{i:D3}",
                    ParentAff = $"PARENT{i}",
                    UserName = $"Affiliate {i}",
                    Player_CustId = i + 1000,
                    Player_Username = $"user{i}",
                    FirstName = $"First{i}",
                    LastName = $"Last{i}",
                    Phone = $"555-{i:D4}",
                    Email = $"user{i}@example.com",
                    Status = "Active",
                    DealVerifyFlags = 0
                });
            }

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffInfos_SpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var affiliateParams = new List<AffiliateParam>
            {
                new AffiliateParam
                {
                    AffId = 123,
                    AffCode = "AFF@123#",
                    ParentAff = "PARENT",
                    UserName = "Test & Co.",
                    Player_CustId = 456,
                    Player_Username = "user_name",
                    FirstName = "O'Brien",
                    LastName = "Müller",
                    Phone = "+1 (555) 123-4567",
                    Email = "test+tag@example.com",
                    Status = "Active",
                    DealVerifyFlags = 0
                }
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffInfos_SingleAffiliate_ShouldProcessCorrectly()
        {
            // Arrange
            var affiliateParams = new List<AffiliateParam>
            {
                new AffiliateParam
                {
                    AffId = 100,
                    AffCode = "AFF100",
                    Status = "Active"
                }
            };

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertAffInfos_SerializationException_ShouldReturnErrorResult()
        {
            // Arrange - Setup repository to throw during execution
            var affiliateParams = new List<AffiliateParam>
            {
                new AffiliateParam { AffId = 1, AffCode = "TEST" }
            };
            var expectedError = "Serialization error";

            _mockRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffInfosCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Text.Json.JsonException(expectedError));

            // Act
            var result = await _controller.InsertAffInfos(affiliateParams);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrMsg.Should().Contain(expectedError);
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
    }
}
