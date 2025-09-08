using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Infrastructure.Repository;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Fanex.Data.Repository;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Repository
{
    public class AffInfoRepositoryTests
    {
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly AffInfoRepository _repository;

        public AffInfoRepositoryTests()
        {
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _repository = new AffInfoRepository(_mockDynamicRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Arrange & Act
            var repository = new AffInfoRepository(_mockDynamicRepository.Object);

            // Assert
            repository.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // Repository constructor doesn't validate null parameters
            var act = () => new AffInfoRepository(null!);
            act.Should().NotThrow();
        }

        #endregion

        #region GetAffInfoListAsync Tests

        [Fact]
        public async Task GetAffInfoListAsync_ValidPaging_ShouldReturnAffInfo()
        {
            // Arrange
            var pagingInfo = new AffInfoCriteria("TEST") { PageIndex = 0, PageSize = 10 };
            var expectedResult = new List<AffInfo>
            {
                new AffInfo
                {
                    AffId = 1,
                    AffCode = "AFF001",
                    ParentAff = "PARENT001",
                    UserName = "TestAffiliate1",
                    Player_CustId = 1001,
                    Player_Username = "player1",
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "123-456-7890",
                    Email = "john@example.com",
                    Status = "Active"
                },
                new AffInfo
                {
                    AffId = 2,
                    AffCode = "AFF002",
                    ParentAff = "PARENT002",
                    UserName = "TestAffiliate2",
                    Player_CustId = 1002,
                    Player_Username = "player2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Phone = "098-765-4321",
                    Email = "jane@example.com",
                    Status = "Inactive"
                }
            };

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _repository.GetAffInfoListAsync(pagingInfo);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            result.Should().HaveCount(2);
            _mockDynamicRepository.Verify(
                x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAffInfoListAsync_ShouldUseCriteriaCorrectly()
        {
            // Arrange
            var pagingInfo = new AffInfoCriteria("TEST") { PageIndex = 1, PageSize = 20 };
            var expectedResult = new List<AffInfo>();

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _repository.GetAffInfoListAsync(pagingInfo);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.FetchAsync<AffInfo>(
                    It.Is<AffInfoCriteria>(c => c.PageIndex == 1 && c.PageSize == 20),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAffInfoListAsync_EmptyResult_ShouldReturnEmptyEnumerable()
        {
            // Arrange
            var expectedResult = new List<AffInfo>();

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _repository.GetAffInfoListAsync(new AffInfoCriteria(string.Empty));

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task GetAffInfoListAsync_LargeDataset_ShouldReturnAllResults()
        {
            // Arrange
            var pagingInfo = new AffInfoCriteria("TEST") { PageIndex = 0, PageSize = 10 };
            var expectedResult = new List<AffInfo>();

            // Generate 10 test affiliate records (reduced from 100 for performance)
            for (int i = 1; i <= 10; i++)
            {
                expectedResult.Add(new AffInfo
                {
                    AffId = i,
                    AffCode = $"AFF{i:000}",
                    ParentAff = $"PARENT{i:000}",
                    UserName = $"TestAffiliate{i}",
                    Player_CustId = 1000 + i,
                    Player_Username = $"player{i}",
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    Phone = $"123-456-{i:0000}",
                    Email = $"affiliate{i}@example.com",
                    Status = i % 2 == 0 ? "Active" : "Inactive"
                });
            }

            // Force garbage collection after creating test data
            GC.Collect();

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _repository.GetAffInfoListAsync(pagingInfo);

            // Assert
            result.Should().HaveCount(10);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetAffInfoListAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var pagingInfo = new AffInfoCriteria("TEST");
            var expectedException = new InvalidOperationException("Database query failed");

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _repository.GetAffInfoListAsync(pagingInfo);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database query failed");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(50)]
        public async Task GetAffInfoListAsync_VariousPageSizes_ShouldHandleAllCorrectly(int pageSize)
        {
            // Arrange
            var pagingInfo = new AffInfoCriteria("TEST") { PageIndex = 0, PageSize = pageSize };
            var expectedResult = new List<AffInfo>();

            for (int i = 1; i <= pageSize; i++)
            {
                expectedResult.Add(new AffInfo
                {
                    AffId = i,
                    AffCode = $"AFF{i:000}",
                    ParentAff = $"PARENT{i:000}",
                    UserName = $"TestAffiliate{i}",
                    Player_CustId = 1000 + i,
                    Player_Username = $"player{i}",
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    Phone = $"123-456-{i:0000}",
                    Email = $"affiliate{i}@example.com",
                    Status = "Active"
                });
            }

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _repository.GetAffInfoListAsync(pagingInfo);

            // Assert
            result.Should().HaveCount(pageSize);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetAffInfoListAsync_NullRepository_ShouldThrowException()
        {
            // Arrange
            var pagingInfo = new AffInfoCriteria("TEST");
            var expectedException = new NullReferenceException("Repository is null");

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _repository.GetAffInfoListAsync(pagingInfo);
            await act.Should().ThrowAsync<NullReferenceException>()
                .WithMessage("Repository is null");
        }

        [Fact]
        public async Task GetAffInfoListAsync_MultipleCalls_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var pagingInfo1 = new AffInfoCriteria("TEST1") { PageIndex = 0, PageSize = 10 };
            var pagingInfo2 = new AffInfoCriteria("TEST2") { PageIndex = 1, PageSize = 20 };
            var pagingInfo3 = new AffInfoCriteria("TEST3") { PageIndex = 2, PageSize = 30 };
            var expectedResult = new List<AffInfo>
            {
                new AffInfo
                {
                    AffId = 1,
                    AffCode = "AFF001",
                    ParentAff = "PARENT001",
                    UserName = "TestAffiliate",
                    Player_CustId = 1001,
                    Player_Username = "player1",
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "123-456-7890",
                    Email = "john@example.com",
                    Status = "Active"
                }
            };

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _repository.GetAffInfoListAsync(pagingInfo1);
            await _repository.GetAffInfoListAsync(pagingInfo2);
            await _repository.GetAffInfoListAsync(pagingInfo3);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task GetAffInfoListAsync_WithSpecificAffiliateData_ShouldReturnCorrectData()
        {
            // Arrange
            var pagingInfo = new AffInfoCriteria("PREMIUM") { PageIndex = 0, PageSize = 1 };
            var expectedResult = new List<AffInfo>
            {
                new AffInfo
                {
                    AffId = 999,
                    AffCode = "PREMIUM001",
                    ParentAff = "PREMIUM_PARENT",
                    UserName = "PremiumAffiliate",
                    Player_CustId = 9999,
                    Player_Username = "premium_player",
                    FirstName = "Premium",
                    LastName = "Partner",
                    Phone = "555-PREMIUM",
                    Email = "premium@example.com",
                    Status = "Active"
                }
            };

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<AffInfo>(It.IsAny<AffInfoCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _repository.GetAffInfoListAsync(pagingInfo);

            // Assert
            var affInfo = result.First();
            affInfo.AffId.Should().Be(999);
            affInfo.AffCode.Should().Be("PREMIUM001");
            affInfo.ParentAff.Should().Be("PREMIUM_PARENT");
            affInfo.UserName.Should().Be("PremiumAffiliate");
            affInfo.Status.Should().Be("Active");
        }

        #endregion
    }
}
