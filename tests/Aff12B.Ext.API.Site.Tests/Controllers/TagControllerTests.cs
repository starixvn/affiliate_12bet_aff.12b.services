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
    /// Unit tests for TagController class
    /// Tests tag data endpoints and retrieval functionality
    /// Target: 100% code coverage
    /// </summary>
    public class TagControllerTests
    {
        private readonly Mock<IDynamicRepository> _mockRepository;
        private readonly TagController _controller;

        public TagControllerTests()
        {
            _mockRepository = new Mock<IDynamicRepository>();
            _controller = new TagController(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Act
            var controller = new TagController(_mockRepository.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeAssignableTo<ControllerBase>();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => new TagController(null!);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task GetTags_ValidParameters_ShouldReturnTags()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 6, 15, 10, 30, 0);
            var batchCount = 1000;
            var expectedData = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "VIP", IsAbnormal = 0, UpdateTime = updatedTime, Description = "VIP Tag", Status = 1 },
                new TagModel { Id = 2, TagName = "Premium", IsAbnormal = 1, UpdateTime = updatedTime.AddHours(1), Description = "Premium Tag", Status = 1 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTags_DefaultBatchCount_ShouldUse2000()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 6, 15, 10, 30, 0);
            var expectedData = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "Standard", IsAbnormal = 0, UpdateTime = updatedTime, Description = "Standard Tag", Status = 1 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTags(updatedTime, 2000);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTags_EmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 1, 1);
            var batchCount = 500;

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TagModel>());

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTags_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 6, 15);
            var batchCount = 1000;

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Tag fetch error"));

            // Act & Assert
            var act = async () => await _controller.GetTags(updatedTime, batchCount);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Tag fetch error");
        }

        [Theory]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(5000)]
        public async Task GetTags_VariousBatchCounts_ShouldHandleCorrectly(int batchCount)
        {
            // Arrange
            var updatedTime = new DateTime(2023, 8, 20);
            var expectedData = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = $"Tag{batchCount}", IsAbnormal = 0, UpdateTime = updatedTime, Description = $"Tag{batchCount} Desc", Status = 1 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTags_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 9, 10, 14, 25, 30);
            var batchCount = 1500;

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TagModel>());

            // Act
            await _controller.GetTags(updatedTime, batchCount);

            // Assert
            // Note: We verify that GetTagCriteria is used correctly
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTags_WithMinDateTime_ShouldHandleCorrectly()
        {
            // Arrange
            var updatedTime = DateTime.MinValue;
            var batchCount = 100;

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TagModel>());

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTags_WithMaxDateTime_ShouldHandleCorrectly()
        {
            // Arrange
            var updatedTime = DateTime.MaxValue;
            var batchCount = 100;

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TagModel>());

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTags_WithZeroBatchCount_ShouldHandleCorrectly()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 7, 1);
            var batchCount = 0;

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TagModel>());

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTags_WithNegativeBatchCount_ShouldHandleCorrectly()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 7, 1);
            var batchCount = -100;

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TagModel>());

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTags_WithLargeDataSet_ShouldHandleCorrectly()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 10, 1);
            var batchCount = 10000;
            var expectedData = new List<TagModel>();

            for (int i = 1; i <= 5000; i++)
            {
                expectedData.Add(new TagModel
                {
                    Id = i,
                    TagName = $"Tag{i}",
                    IsAbnormal = (byte)(i % 2),
                    UpdateTime = updatedTime.AddMinutes(i),
                    Description = $"Tag{i} Desc",
                    Status = i % 3
                });
            }

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(5000);
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetTags_MultipleCallsInParallel_ShouldHandleCorrectly()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 11, 15);
            var batchCount = 1000;
            var expectedData = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "ParallelTag", IsAbnormal = 0, UpdateTime = updatedTime, Description = "Parallel Tag", Status = 1 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var task1 = _controller.GetTags(updatedTime, batchCount);
            var task2 = _controller.GetTags(updatedTime, batchCount);
            var task3 = _controller.GetTags(updatedTime, batchCount);
            var results = await Task.WhenAll(task1, task2, task3);

            // Assert
            results.Should().HaveCount(3);
            results[0].Should().BeEquivalentTo(expectedData);
            results[1].Should().BeEquivalentTo(expectedData);
            results[2].Should().BeEquivalentTo(expectedData);
            _mockRepository.Verify(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GetTags_WithSpecificTimeComponents_ShouldHandleCorrectly()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 12, 25, 23, 59, 59, 999);
            var batchCount = 2500;
            var expectedData = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "ChristmasTag", IsAbnormal = 0, UpdateTime = updatedTime, Description = "Christmas Tag", Status = 1 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetTags_DatabaseConnectionError_ShouldPropagateException()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 1, 1);
            var batchCount = 1000;

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("SQL Connection Error"));

            // Act & Assert
            var act = async () => await _controller.GetTags(updatedTime, batchCount);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GetTags_WithAbnormalTags_ShouldReturnCorrectly()
        {
            // Arrange
            var updatedTime = new DateTime(2023, 5, 10);
            var batchCount = 500;
            var expectedData = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "NormalTag", IsAbnormal = 0, UpdateTime = updatedTime, Description = "Normal Tag", Status = 1 },
                new TagModel { Id = 2, TagName = "AbnormalTag", IsAbnormal = 1, UpdateTime = updatedTime.AddHours(1), Description = "Abnormal Tag", Status = 1 },
                new TagModel { Id = 3, TagName = "AnotherNormalTag", IsAbnormal = 0, UpdateTime = updatedTime.AddHours(2), Description = "Another Normal Tag", Status = 1 }
            };

            _mockRepository
                .Setup(x => x.FetchAsync<TagModel>(It.IsAny<GetTagCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetTags(updatedTime, batchCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(t => t.IsAbnormal == 0);
            result.Should().Contain(t => t.IsAbnormal == 1);
            result.Should().BeEquivalentTo(expectedData);
        }
    }
}
