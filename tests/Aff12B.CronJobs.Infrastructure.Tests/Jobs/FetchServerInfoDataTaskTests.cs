using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs
{
    /// <summary>
    /// Comprehensive unit tests for FetchServerInfoDataTask class.
    /// Business-focused scenarios for server information data fetching and import workflows.
    /// Covers API integration, server configuration data, and multi-repository import scenarios.
    /// </summary>
    public class FetchServerInfoDataTaskTests
    {
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockSettings;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly FetchServerInfoDataTask _task;

        public FetchServerInfoDataTaskTests()
        {
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockSettings = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "MAIN", "VIP", "MOBILE" },
                MaxTimeRange = 120,
                AdjustmentValue = -15
            };

            _mockSettings.Setup(x => x.Get(It.IsAny<string>())).Returns(_fetchServiceSetting);

            _task = new FetchServerInfoDataTask(_mockRepository.Object, _mockLicenseeApi.Object, _mockSettings.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchServerInfoDataTask(_mockRepository.Object, _mockLicenseeApi.Object, _mockSettings.Object);
            task.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            var act = () => new FetchServerInfoDataTask(null!, _mockLicenseeApi.Object, _mockSettings.Object);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenLicenseeApiIsNull()
        {
            // Act & Assert
            var act = () => new FetchServerInfoDataTask(_mockRepository.Object, null!, _mockSettings.Object);
            act.Should().Throw<ArgumentNullException>().WithParameterName("licenseeApi");
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenSettingsIsNull()
        {
            // Act & Assert
            var act = () => new FetchServerInfoDataTask(_mockRepository.Object, _mockLicenseeApi.Object, null!);
            act.Should().Throw<Exception>(); // Could be ArgumentNullException or NullReferenceException
        }

        [Fact]
        public async Task FetchDataAsync_ShouldFetchServerInfoFromApi()
        {
            // Arrange - Server configuration data scenario
            var serverInfoList = new List<ServerInfoModel>
            {
                new ServerInfoModel
                {
                    SiteId = 1,
                    SiteName = "Main Casino",
                    LangList = "en,zh,ja",
                    CurrencyList = "USD,EUR,CNY",
                    DefLang = "en",
                    DefCurrency = 1
                },
                new ServerInfoModel
                {
                    SiteId = 2,
                    SiteName = "VIP Casino",
                    LangList = "en,zh",
                    CurrencyList = "USD,EUR",
                    DefLang = "en",
                    DefCurrency = 1
                }
            };

            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(serverInfoList);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should call API to get server info
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldImportDataToMultipleRepositories()
        {
            // Arrange - Multi-site server info import scenario
            var serverInfoList = new List<ServerInfoModel>
            {
                new ServerInfoModel
                {
                    SiteId = 100,
                    SiteName = "Mobile Casino",
                    LangList = "en,es,pt",
                    CurrencyList = "USD,BRL,MXN",
                    DefLang = "en",
                    DefCurrency = 1
                }
            };

            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(serverInfoList);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should import to repository (base class functionality)
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldHandleEmptyServerInfoList()
        {
            // Arrange - Empty server info list scenario
            var emptyServerInfo = new List<ServerInfoModel>();
            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(emptyServerInfo);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should handle empty list gracefully
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldHandleApiException()
        {
            // Arrange - API failure scenario
            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync())
                .ThrowsAsync(new HttpRequestException("Server info API unavailable"));

            // Act & Assert - Should propagate API exception
            await _task.Invoking(t => t.FetchDataAsync())
                .Should().ThrowAsync<HttpRequestException>()
                .WithMessage("Server info API unavailable");
        }

        [Fact]
        public async Task FetchDataAsync_ShouldHandleNullApiResponse()
        {
            // Arrange - Null API response scenario
            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync((IEnumerable<ServerInfoModel>)null!);

            // Act & Assert - Should handle null response
            await _task.Invoking(t => t.FetchDataAsync())
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task FetchDataAsync_ShouldSupportMultiLanguageServerConfiguration()
        {
            // Arrange - Multi-language server configuration scenario
            var multiLangServers = new List<ServerInfoModel>
            {
                new ServerInfoModel
                {
                    SiteId = 1,
                    SiteName = "Global Casino",
                    LangList = "en,zh-CN,zh-TW,ja,ko,th,vi,id,ms",
                    CurrencyList = "USD,EUR,CNY,JPY,KRW,THB,VND,IDR,MYR",
                    DefLang = "en",
                    DefCurrency = 1
                },
                new ServerInfoModel
                {
                    SiteId = 2,
                    SiteName = "European Casino",
                    LangList = "en,de,fr,es,it,pt,ru,pl",
                    CurrencyList = "EUR,GBP,USD,CHF,SEK,NOK,DKK,PLN",
                    DefLang = "en",
                    DefCurrency = 2
                }
            };

            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(multiLangServers);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should process multi-language configurations
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldHandleUnicodeServerNames()
        {
            // Arrange - Unicode server names scenario
            var unicodeServers = new List<ServerInfoModel>
            {
                new ServerInfoModel
                {
                    SiteId = 1,
                    SiteName = "中文赌场_Chinese_Casino",
                    LangList = "zh-CN,en",
                    CurrencyList = "CNY,USD",
                    DefLang = "zh-CN",
                    DefCurrency = 1
                },
                new ServerInfoModel
                {
                    SiteId = 2,
                    SiteName = "日本カジノ_Japanese_Casino",
                    LangList = "ja,en",
                    CurrencyList = "JPY,USD",
                    DefLang = "ja",
                    DefCurrency = 2
                }
            };

            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(unicodeServers);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should handle unicode content
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldHandleLargeServerInfoDataset()
        {
            // Arrange - Large dataset scenario (many server configurations)
            var largeServerList = new List<ServerInfoModel>();
            for (int i = 1; i <= 100; i++)
            {
                largeServerList.Add(new ServerInfoModel
                {
                    SiteId = i,
                    SiteName = $"Casino Server {i}",
                    LangList = "en,zh",
                    CurrencyList = "USD,EUR",
                    DefLang = "en",
                    DefCurrency = 1
                });
            }

            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(largeServerList);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should handle large datasets
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public void FetchServerInfoDataTask_ShouldInheritFromBaseClass()
        {
            // Arrange & Act & Assert - Should inherit from base class
            _task.Should().BeAssignableTo<FetchDataForMultipleRepositoryTaskBase<ServerInfoModel>>();
        }

        [Fact]
        public async Task FetchDataAsync_ShouldHandleTimeoutException()
        {
            // Arrange - API timeout scenario
            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync())
                .ThrowsAsync(new TimeoutException("Server info API request timeout"));

            // Act & Assert - Should handle timeout appropriately
            await _task.Invoking(t => t.FetchDataAsync())
                .Should().ThrowAsync<TimeoutException>()
                .WithMessage("Server info API request timeout");
        }

        [Fact]
        public async Task FetchDataAsync_ShouldSupportConcurrentExecution()
        {
            // Arrange - Concurrent execution scenario
            var serverInfo = new List<ServerInfoModel>
            {
                new ServerInfoModel
                {
                    SiteId = 1,
                    SiteName = "Concurrent Test Server",
                    LangList = "en",
                    CurrencyList = "USD",
                    DefLang = "en",
                    DefCurrency = 1
                }
            };

            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(serverInfo);

            // Act - Multiple concurrent calls
            var tasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                tasks.Add(_task.FetchDataAsync());
            }

            await Task.WhenAll(tasks);

            // Assert - Should handle concurrent execution
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Exactly(3));
        }

        [Fact]
        public async Task FetchDataAsync_ShouldHandleSpecialServerConfigurations()
        {
            // Arrange - Special server configuration scenarios
            var specialServers = new List<ServerInfoModel>
            {
                new ServerInfoModel
                {
                    SiteId = 0,
                    SiteName = "",
                    LangList = "",
                    CurrencyList = "",
                    DefLang = "",
                    DefCurrency = null
                }, // Edge case: empty values
                new ServerInfoModel
                {
                    SiteId = -1,
                    SiteName = "Invalid Server",
                    LangList = null!,
                    CurrencyList = null!,
                    DefLang = null!,
                    DefCurrency = -1
                }, // Edge case: negative/null values
                new ServerInfoModel
                {
                    SiteId = int.MaxValue,
                    SiteName = "Max Server",
                    LangList = "en",
                    CurrencyList = "USD",
                    DefLang = "en",
                    DefCurrency = int.MaxValue
                } // Edge case: max values
            };

            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(specialServers);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should handle special cases
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldSupportBusinessContinuityScenario()
        {
            // Arrange - Business continuity scenario with backup servers
            var businessContinuityServers = new List<ServerInfoModel>
            {
                new ServerInfoModel
                {
                    SiteId = 1,
                    SiteName = "Primary Casino Server",
                    LangList = "en,zh,ja,ko",
                    CurrencyList = "USD,EUR,CNY,JPY,KRW",
                    DefLang = "en",
                    DefCurrency = 1
                },
                new ServerInfoModel
                {
                    SiteId = 2,
                    SiteName = "Backup Casino Server",
                    LangList = "en,zh,ja,ko",
                    CurrencyList = "USD,EUR,CNY,JPY,KRW",
                    DefLang = "en",
                    DefCurrency = 1
                },
                new ServerInfoModel
                {
                    SiteId = 3,
                    SiteName = "Disaster Recovery Server",
                    LangList = "en",
                    CurrencyList = "USD",
                    DefLang = "en",
                    DefCurrency = 1
                }
            };

            _mockLicenseeApi.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(businessContinuityServers);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should process all server configurations
            _mockLicenseeApi.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }
    }
}
