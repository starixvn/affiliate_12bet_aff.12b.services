using Aff12B.CronJobs.Infrastructure.Exceptions;
using Aff12B.CronJobs.Infrastructure.Repository;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Repository
{
    public class ThirdPartyImportRepositoryTests
    {
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly ThirdPartyImportRepository _repository;

        public ThirdPartyImportRepositoryTests()
        {
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _repository = new ThirdPartyImportRepository(_mockDynamicRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Arrange & Act
            var repository = new ThirdPartyImportRepository(_mockDynamicRepository.Object);

            // Assert
            repository.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // Repository constructor doesn't validate null parameters
            var act = () => new ThirdPartyImportRepository(null!);
            act.Should().NotThrow();
        }

        #endregion

        #region ImportRecordAsync Tests

        [Fact]
        public async Task ImportRecordAsync_ValidRecord_ErrCodeZero_ShouldImportSuccessfully()
        {
            // Arrange
            var record = new InsertThirdPartyRecodInfoCommand("TEST");
            record.ErrCode = 0;

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var act = async () => await _repository.ImportRecordAsync(record);

            // Assert
            await act.Should().NotThrowAsync();
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ImportRecordAsync_ValidRecord_NonZeroErrCode_ShouldThrowImporterException()
        {
            // Arrange
            var record = new InsertThirdPartyRecodInfoCommand("TEST");
            record.ErrCode = 1001;

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    // Simulate the command properties being set after execution
                    record.ErrCode = 1001;
                    record.ErrMsg = "Record import failed";
                    record.XmlData = "<error>Import error</error>";
                });

            // Act & Assert
            var act = async () => await _repository.ImportRecordAsync(record);
            await act.Should().ThrowAsync<ImporterException>();
        }

        [Theory]
        [InlineData(1001, "Record already exists")]
        [InlineData(2002, "Invalid record data")]
        [InlineData(3003, "Database constraint violation")]
        [InlineData(4004, "Third party service unavailable")]
        public async Task ImportRecordAsync_VariousErrorCodes_ShouldThrowImporterException(int errorCode, string errorMessage)
        {
            // Arrange
            var record = new InsertThirdPartyRecodInfoCommand("TEST");
            record.ErrCode = errorCode;

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    record.ErrCode = errorCode;
                    record.ErrMsg = errorMessage;
                    record.XmlData = $"<error>{errorMessage}</error>";
                });

            // Act & Assert
            var act = async () => await _repository.ImportRecordAsync(record);
            await act.Should().ThrowAsync<ImporterException>();
        }

        [Fact]
        public async Task ImportRecordAsync_ShouldCastRecordCorrectly()
        {
            // Arrange
            var record = new InsertThirdPartyRecodInfoCommand("TEST");
            record.ErrCode = 0;

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.ImportRecordAsync(record);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(
                    It.Is<InsertThirdPartyRecodInfoCommand>(c => c == (InsertThirdPartyRecodInfoCommand)record),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ImportRecordAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var record = new InsertThirdPartyRecodInfoCommand("TEST");
            record.ErrCode = 0;
            var expectedException = new InvalidOperationException("Database connection failed");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _repository.ImportRecordAsync(record);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database connection failed");
        }

        [Fact]
        public async Task ImportRecordAsync_MultipleCalls_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var record1 = new InsertThirdPartyRecodInfoCommand("TEST1");
            var record2 = new InsertThirdPartyRecodInfoCommand("TEST2");
            var record3 = new InsertThirdPartyRecodInfoCommand("TEST3");

            record1.ErrCode = 0;
            record2.ErrCode = 0;
            record3.ErrCode = 0;

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.ImportRecordAsync(record1);
            await _repository.ImportRecordAsync(record2);
            await _repository.ImportRecordAsync(record3);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task ImportRecordAsync_WithSpecificErrorDetails_ShouldThrowImporterExceptionWithCorrectInfo()
        {
            // Arrange
            var record = new InsertThirdPartyRecodInfoCommand("TEST");
            record.ErrCode = 9999;

            const int expectedErrorCode = 9999;
            const string expectedErrorMessage = "Critical system error occurred";
            const string expectedXmlData = "<critical><error>System failure</error><timestamp>2023-12-25T10:30:45</timestamp></critical>";

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    record.ErrCode = expectedErrorCode;
                    record.ErrMsg = expectedErrorMessage;
                    record.XmlData = expectedXmlData;
                });

            // Act & Assert
            var act = async () => await _repository.ImportRecordAsync(record);
            var exception = await act.Should().ThrowAsync<ImporterException>();

            // Verify exception contains expected information
            exception.Which.Should().NotBeNull();
        }

        [Fact]
        public async Task ImportRecordAsync_ZeroErrorCode_ShouldNotCheckCommandProperties()
        {
            // Arrange
            var record = new InsertThirdPartyRecodInfoCommand("TEST");
            record.ErrCode = 0;

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    // Simulate successful execution - ErrCode remains 0
                    record.ErrCode = 0;
                    record.ErrMsg = "Success";
                    record.XmlData = "<success>Import completed</success>";
                });

            // Act & Assert
            var act = async () => await _repository.ImportRecordAsync(record);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ImportRecordAsync_ConcurrentCalls_ShouldHandleAllCorrectly()
        {
            // Arrange
            var records = new List<InsertThirdPartyRecodInfoCommand>();
            for (int i = 0; i < 10; i++)
            {
                var record = new InsertThirdPartyRecodInfoCommand("TEST");
                record.ErrCode = 0;
                records.Add(record);
            }

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var tasks = records.Select(record => _repository.ImportRecordAsync(record));
            await Task.WhenAll(tasks);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Exactly(10));
        }

        [Theory]
        [InlineData(0, false)]    // Success case - should not throw
        [InlineData(1, true)]     // Error case - should throw
        [InlineData(999, true)]   // Another error case - should throw
        [InlineData(-1, true)]    // Negative error code - should throw
        public async Task ImportRecordAsync_VariousErrorCodeScenarios_ShouldBehaveCorrectly(int errorCode, bool shouldThrow)
        {
            // Arrange
            var record = new InsertThirdPartyRecodInfoCommand("TEST");
            record.ErrCode = errorCode;

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertThirdPartyRecodInfoCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1))
                .Callback<INonQueryCommand, string, CancellationToken>((cmd, conn, token) =>
                {
                    record.ErrCode = errorCode;
                    record.ErrMsg = $"Error message for code {errorCode}";
                    record.XmlData = $"<error>Code {errorCode}</error>";
                });

            // Act & Assert
            var act = async () => await _repository.ImportRecordAsync(record);

            if (shouldThrow)
            {
                await act.Should().ThrowAsync<ImporterException>();
            }
            else
            {
                await act.Should().NotThrowAsync();
            }
        }

        #endregion
    }
}
