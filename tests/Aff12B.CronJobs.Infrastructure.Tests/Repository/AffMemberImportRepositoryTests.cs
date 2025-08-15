using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Exceptions;
using Aff12B.CronJobs.Infrastructure.Repository;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Repository
{
    public class AffMemberImportRepositoryTests
    {
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly AffMemberImportRepository _repository;

        public AffMemberImportRepositoryTests()
        {
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _repository = new AffMemberImportRepository(_mockDynamicRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Arrange & Act
            var repository = new AffMemberImportRepository(_mockDynamicRepository.Object);

            // Assert
            repository.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // Repository constructor doesn't validate null parameters
            var act = () => new AffMemberImportRepository(null!);
            act.Should().NotThrow();
        }

        #endregion

        #region GetLastedMemberImportedTimeAsync Tests

        [Fact]
        public async Task GetLastedMemberImportedTimeAsync_ValidSetting_ShouldReturnDateTime()
        {
            // Arrange
            var setting = new MemberImportedTimeInfo("TEST", "TestService");
            var expectedDateTime = new DateTime(2023, 12, 25, 10, 30, 45);

            _mockDynamicRepository
                .Setup(x => x.GetAsync<DateTime>(It.IsAny<MemberImportedTimeInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDateTime);

            // Act
            var result = await _repository.GetLastedMemberImportedTimeAsync(setting);

            // Assert
            result.Should().Be(expectedDateTime);
            _mockDynamicRepository.Verify(
                x => x.GetAsync<DateTime>(It.IsAny<MemberImportedTimeInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetLastedMemberImportedTimeAsync_ShouldCastSettingCorrectly()
        {
            // Arrange
            var setting = new MemberImportedTimeInfo("TEST", "TestService");
            var expectedDateTime = DateTime.Now;

            _mockDynamicRepository
                .Setup(x => x.GetAsync<DateTime>(It.IsAny<MemberImportedTimeInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDateTime);

            // Act
            await _repository.GetLastedMemberImportedTimeAsync(setting);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.GetAsync<DateTime>(
                    It.Is<MemberImportedTimeInfo>(m => m == (MemberImportedTimeInfo)setting),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetLastedMemberImportedTimeAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var setting = new MemberImportedTimeInfo("TEST", "TestService");
            var expectedException = new InvalidOperationException("Database connection failed");

            _mockDynamicRepository
                .Setup(x => x.GetAsync<DateTime>(It.IsAny<MemberImportedTimeInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _repository.GetLastedMemberImportedTimeAsync(setting);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database connection failed");
        }

        #endregion

        #region GetSiteNameAsync Tests

        [Fact]
        public async Task GetSiteNameAsync_ValidSetting_ShouldReturnSiteName()
        {
            // Arrange
            var setting = new SiteNameInfo("TEST");
            const string expectedSiteName = "TestSite";

            _mockDynamicRepository
                .Setup(x => x.GetAsync<string>(It.IsAny<SiteNameInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSiteName);

            // Act
            var result = await _repository.GetSiteNameAsync(setting);

            // Assert
            result.Should().Be(expectedSiteName);
            _mockDynamicRepository.Verify(
                x => x.GetAsync<string>(It.IsAny<SiteNameInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetSiteNameAsync_ShouldCastSettingCorrectly()
        {
            // Arrange
            var setting = new SiteNameInfo("TEST");
            const string expectedSiteName = "AnotherSite";

            _mockDynamicRepository
                .Setup(x => x.GetAsync<string>(It.IsAny<SiteNameInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSiteName);

            // Act
            await _repository.GetSiteNameAsync(setting);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.GetAsync<string>(
                    It.Is<SiteNameInfo>(s => s == (SiteNameInfo)setting),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task GetSiteNameAsync_EmptyOrNullResult_ShouldReturnValue(string siteName)
        {
            // Arrange
            var setting = new SiteNameInfo("TEST");

            _mockDynamicRepository
                .Setup(x => x.GetAsync<string>(It.IsAny<SiteNameInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(siteName!);

            // Act
            var result = await _repository.GetSiteNameAsync(setting);

            // Assert
            result.Should().Be(siteName);
        }

        #endregion

        #region ImportMemberAsync Tests

        [Fact]
        public async Task ImportMemberAsync_ValidMember_ErrCodeZero_ShouldImportSuccessfully()
        {
            // Arrange
            var memberInfo = new InsertMemberInfoCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertMemberInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    // Simulate successful execution - set ErrCode on the actual object
                    memberInfo.ErrCode = 0;
                });

            // Act
            var act = async () => await _repository.ImportMemberAsync(memberInfo);

            // Assert
            await act.Should().NotThrowAsync();
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertMemberInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ImportMemberAsync_ValidMember_NonZeroErrCode_ShouldThrowImporterException()
        {
            // Arrange
            var memberInfo = new InsertMemberInfoCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertMemberInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    // Simulate error
                    memberInfo.ErrCode = 1001;
                    memberInfo.ErrMsg = "Member already exists";
                    memberInfo.XmlData = "<error>Member duplicate</error>";
                });

            // Act & Assert
            var act = async () => await _repository.ImportMemberAsync(memberInfo);
            await act.Should().ThrowAsync<ImporterException>();
        }

        [Theory]
        [InlineData(1001, "Member already exists")]
        [InlineData(2002, "Invalid member data")]
        [InlineData(3003, "Database constraint violation")]
        public async Task ImportMemberAsync_VariousErrorCodes_ShouldThrowImporterException(int errorCode, string errorMessage)
        {
            // Arrange
            var memberInfo = new InsertMemberInfoCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertMemberInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    memberInfo.ErrCode = errorCode;
                    memberInfo.ErrMsg = errorMessage;
                    memberInfo.XmlData = $"<error>{errorMessage}</error>";
                });

            // Act & Assert
            var act = async () => await _repository.ImportMemberAsync(memberInfo);
            await act.Should().ThrowAsync<ImporterException>();
        }

        [Fact]
        public async Task ImportMemberAsync_ShouldCastMemberInfoCorrectly()
        {
            // Arrange
            var memberInfo = new InsertMemberInfoCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertMemberInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    memberInfo.ErrCode = 0;
                });

            // Act
            await _repository.ImportMemberAsync(memberInfo);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(
                    It.Is<InsertMemberInfoCommand>(c => c == (InsertMemberInfoCommand)memberInfo),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region InsertAffMemberTimeTrackerAsync Tests

        [Fact]
        public async Task InsertAffMemberTimeTrackerAsync_Success_ShouldComplete()
        {
            // Arrange
            var timeTracker = new InsertAffMemberTimeTrackerCommand("TEST", "TestService");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffMemberTimeTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    timeTracker.ErrCode = SqlResponseCode.Success;
                });

            // Act
            var act = async () => await _repository.InsertAffMemberTimeTrackerAsync(timeTracker);

            // Assert
            await act.Should().NotThrowAsync();
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertAffMemberTimeTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task InsertAffMemberTimeTrackerAsync_NonSuccessErrCode_ShouldThrowImporterException()
        {
            // Arrange
            var timeTracker = new InsertAffMemberTimeTrackerCommand("TEST", "TestService");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffMemberTimeTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    timeTracker.ErrCode = 1001;
                    timeTracker.ErrMsg = "Time tracker insert failed";
                });

            // Act & Assert
            var act = async () => await _repository.InsertAffMemberTimeTrackerAsync(timeTracker);
            await act.Should().ThrowAsync<ImporterException>();
        }

        [Theory]
        [InlineData(1001, "Time tracker already exists")]
        [InlineData(2002, "Invalid time tracker data")]
        [InlineData(3003, "Foreign key constraint violation")]
        public async Task InsertAffMemberTimeTrackerAsync_VariousErrorCodes_ShouldThrowImporterException(int errorCode, string errorMessage)
        {
            // Arrange
            var timeTracker = new InsertAffMemberTimeTrackerCommand("TEST", "TestService");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffMemberTimeTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    timeTracker.ErrCode = errorCode;
                    timeTracker.ErrMsg = errorMessage;
                });

            // Act & Assert
            var act = async () => await _repository.InsertAffMemberTimeTrackerAsync(timeTracker);
            await act.Should().ThrowAsync<ImporterException>();
        }

        [Fact]
        public async Task InsertAffMemberTimeTrackerAsync_ShouldCastTimeTrackerCorrectly()
        {
            // Arrange
            var timeTracker = new InsertAffMemberTimeTrackerCommand("TEST", "TestService");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffMemberTimeTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    timeTracker.ErrCode = SqlResponseCode.Success;
                });

            // Act
            await _repository.InsertAffMemberTimeTrackerAsync(timeTracker);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(
                    It.Is<InsertAffMemberTimeTrackerCommand>(c => c == (InsertAffMemberTimeTrackerCommand)timeTracker),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Repository_CompleteWorkflow_ShouldExecuteAllOperations()
        {
            // Arrange
            var memberTimeSetting = new MemberImportedTimeInfo("TEST", "TestService");
            var siteNameSetting = new SiteNameInfo("TEST");
            var memberInfo = new InsertMemberInfoCommand("TEST");
            var timeTracker = new InsertAffMemberTimeTrackerCommand("TEST", "TestService");

            var expectedDateTime = new DateTime(2023, 12, 25, 10, 30, 45);
            const string expectedSiteName = "TestSite";

            _mockDynamicRepository
                .Setup(x => x.GetAsync<DateTime>(It.IsAny<MemberImportedTimeInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDateTime);

            _mockDynamicRepository
                .Setup(x => x.GetAsync<string>(It.IsAny<SiteNameInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSiteName);

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertMemberInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) => memberInfo.ErrCode = 0);

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertAffMemberTimeTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) => timeTracker.ErrCode = SqlResponseCode.Success);

            // Act
            var lastImportTime = await _repository.GetLastedMemberImportedTimeAsync(memberTimeSetting);
            var siteName = await _repository.GetSiteNameAsync(siteNameSetting);
            await _repository.ImportMemberAsync(memberInfo);
            await _repository.InsertAffMemberTimeTrackerAsync(timeTracker);

            // Assert
            lastImportTime.Should().Be(expectedDateTime);
            siteName.Should().Be(expectedSiteName);

            _mockDynamicRepository.Verify(
                x => x.GetAsync<DateTime>(It.IsAny<MemberImportedTimeInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDynamicRepository.Verify(
                x => x.GetAsync<string>(It.IsAny<SiteNameInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertMemberInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertAffMemberTimeTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion
    }
}
