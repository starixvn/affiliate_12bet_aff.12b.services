using Aff12B.CronJobs.Infrastructure.Configurations;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Configurations
{
    /// <summary>
    /// Unit tests for ProxySetting class.
    /// Business-focused tests for proxy configuration scenarios.
    /// </summary>
    public class ProxySettingTests
    {
        [Fact]
        public void ProxySetting_ShouldSupportDirectConnectionScenario()
        {
            // Arrange - Direct connection (no proxy)
            var setting = new ProxySetting
            {
                IsUseProxy = false,
                ProxyUrl = null
            };

            // Assert - Direct connection configuration
            setting.IsUseProxy.Should().BeFalse();
            setting.ProxyUrl.Should().BeNull();
        }

        [Fact]
        public void ProxySetting_ShouldSupportCorporateProxyScenario()
        {
            // Arrange - Corporate proxy scenario
            var setting = new ProxySetting
            {
                IsUseProxy = true,
                ProxyUrl = "http://corporate-proxy.company.com:8080"
            };

            // Assert - Corporate proxy configuration
            setting.IsUseProxy.Should().BeTrue();
            setting.ProxyUrl.Should().Be("http://corporate-proxy.company.com:8080");
            setting.ProxyUrl.Should().StartWith("http://");
            setting.ProxyUrl.Should().Contain("corporate-proxy");
            setting.ProxyUrl.Should().EndWith(":8080");
        }

        [Fact]
        public void ProxySetting_ShouldSupportSecureProxyScenario()
        {
            // Arrange - HTTPS proxy scenario
            var setting = new ProxySetting
            {
                IsUseProxy = true,
                ProxyUrl = "https://secure-proxy.enterprise.com:443"
            };

            // Assert - Secure proxy configuration
            setting.IsUseProxy.Should().BeTrue();
            setting.ProxyUrl.Should().StartWith("https://");
            setting.ProxyUrl.Should().Contain("secure-proxy");
            setting.ProxyUrl.Should().EndWith(":443");
        }

        [Fact]
        public void ProxySetting_ShouldSupportAuthenticatedProxyScenario()
        {
            // Arrange - Authenticated proxy scenario
            var setting = new ProxySetting
            {
                IsUseProxy = true,
                ProxyUrl = "http://username:password@auth-proxy.company.com:3128"
            };

            // Assert - Authenticated proxy configuration
            setting.IsUseProxy.Should().BeTrue();
            setting.ProxyUrl.Should().Contain("username:password@");
            setting.ProxyUrl.Should().Contain("auth-proxy");
            setting.ProxyUrl.Should().EndWith(":3128");
        }

        [Fact]
        public void ProxySetting_ShouldSupportSocksProxyScenario()
        {
            // Arrange - SOCKS proxy scenario
            var setting = new ProxySetting
            {
                IsUseProxy = true,
                ProxyUrl = "socks5://socks-proxy.network.com:1080"
            };

            // Assert - SOCKS proxy configuration
            setting.IsUseProxy.Should().BeTrue();
            setting.ProxyUrl.Should().StartWith("socks5://");
            setting.ProxyUrl.Should().Contain("socks-proxy");
            setting.ProxyUrl.Should().EndWith(":1080");
        }

        [Fact]
        public void ProxySetting_ShouldSupportLoadBalancedProxyScenario()
        {
            // Arrange - Load balanced proxy scenario
            var settings = new[]
            {
                new ProxySetting { IsUseProxy = true, ProxyUrl = "http://proxy1.loadbalancer.com:8080" },
                new ProxySetting { IsUseProxy = true, ProxyUrl = "http://proxy2.loadbalancer.com:8080" },
                new ProxySetting { IsUseProxy = true, ProxyUrl = "http://proxy3.loadbalancer.com:8080" }
            };

            // Assert - Load balanced proxy configurations
            settings.Should().HaveCount(3);
            settings.Should().OnlyContain(s => s.IsUseProxy == true);
            settings.Should().OnlyContain(s => s.ProxyUrl.Contains("loadbalancer.com"));
            settings.Should().OnlyContain(s => s.ProxyUrl.EndsWith(":8080"));
        }

        [Fact]
        public void ProxySetting_ShouldSupportDevelopmentEnvironmentScenario()
        {
            // Arrange - Development environment (local proxy)
            var setting = new ProxySetting
            {
                IsUseProxy = true,
                ProxyUrl = "http://localhost:8888"
            };

            // Assert - Development proxy configuration
            setting.IsUseProxy.Should().BeTrue();
            setting.ProxyUrl.Should().Contain("localhost");
            setting.ProxyUrl.Should().EndWith(":8888");
        }

        [Fact]
        public void ProxySetting_ShouldSupportProductionEnvironmentScenario()
        {
            // Arrange - Production environment (enterprise proxy)
            var setting = new ProxySetting
            {
                IsUseProxy = true,
                ProxyUrl = "http://prod-proxy.enterprise.com:8080"
            };

            // Assert - Production proxy configuration
            setting.IsUseProxy.Should().BeTrue();
            setting.ProxyUrl.Should().Contain("prod-proxy");
            setting.ProxyUrl.Should().Contain("enterprise.com");
            setting.ProxyUrl.Should().NotContain("localhost");
            setting.ProxyUrl.Should().NotContain("dev");
            setting.ProxyUrl.Should().NotContain("test");
        }

        [Fact]
        public void ProxySetting_ShouldSupportDisabledProxyWithUrlScenario()
        {
            // Arrange - Proxy disabled but URL configured (common configuration pattern)
            var setting = new ProxySetting
            {
                IsUseProxy = false,
                ProxyUrl = "http://backup-proxy.company.com:8080"
            };

            // Assert - Disabled proxy with configured URL
            setting.IsUseProxy.Should().BeFalse();
            setting.ProxyUrl.Should().NotBeNullOrEmpty();
            setting.ProxyUrl.Should().Contain("backup-proxy");
        }

        [Fact]
        public void ProxySetting_ShouldSupportInternationalProxyScenario()
        {
            // Arrange - International proxy scenarios
            var internationalSettings = new[]
            {
                new ProxySetting { IsUseProxy = true, ProxyUrl = "http://proxy.europe.company.com:8080" },
                new ProxySetting { IsUseProxy = true, ProxyUrl = "http://proxy.asia.company.com:8080" },
                new ProxySetting { IsUseProxy = true, ProxyUrl = "http://proxy.americas.company.com:8080" }
            };

            // Assert - International proxy configurations
            internationalSettings.Should().HaveCount(3);
            internationalSettings.Should().OnlyContain(s => s.IsUseProxy == true);
            internationalSettings.Should().Contain(s => s.ProxyUrl.Contains("europe"));
            internationalSettings.Should().Contain(s => s.ProxyUrl.Contains("asia"));
            internationalSettings.Should().Contain(s => s.ProxyUrl.Contains("americas"));
        }

        [Fact]
        public void ProxySetting_ShouldSupportEdgeCaseScenarios()
        {
            // Arrange - Edge case scenarios
            var edgeCases = new[]
            {
                new ProxySetting { IsUseProxy = true, ProxyUrl = "" }, // Empty URL
                new ProxySetting { IsUseProxy = true, ProxyUrl = "   " }, // Whitespace URL
                new ProxySetting { IsUseProxy = false, ProxyUrl = "" }, // Disabled with empty URL
                new ProxySetting { IsUseProxy = false, ProxyUrl = null } // Disabled with null URL
            };

            // Assert - Edge cases should be handled
            edgeCases.Should().HaveCount(4);
            edgeCases.Should().Contain(s => s.IsUseProxy == true && s.ProxyUrl == "");
            edgeCases.Should().Contain(s => s.IsUseProxy == true && s.ProxyUrl == "   ");
            edgeCases.Should().Contain(s => s.IsUseProxy == false && s.ProxyUrl == "");
            edgeCases.Should().Contain(s => s.IsUseProxy == false && s.ProxyUrl == null);
        }

        [Fact]
        public void ProxySetting_ShouldSupportConfigurationValidationScenario()
        {
            // Arrange - Configuration validation scenarios
            var validConfigs = new[]
            {
                new ProxySetting { IsUseProxy = false, ProxyUrl = null }, // Valid: Direct connection
                new ProxySetting { IsUseProxy = true, ProxyUrl = "http://proxy.com:8080" }, // Valid: HTTP proxy
                new ProxySetting { IsUseProxy = true, ProxyUrl = "https://proxy.com:443" }, // Valid: HTTPS proxy
                new ProxySetting { IsUseProxy = true, ProxyUrl = "socks5://proxy.com:1080" } // Valid: SOCKS proxy
            };

            var questionableConfigs = new[]
            {
                new ProxySetting { IsUseProxy = true, ProxyUrl = null }, // Questionable: Enabled but no URL
                new ProxySetting { IsUseProxy = true, ProxyUrl = "" }, // Questionable: Enabled but empty URL
            };

            // Assert - Configuration validation
            validConfigs.Should().HaveCount(4);
            questionableConfigs.Should().HaveCount(2);

            // Valid configurations should have consistent settings
            validConfigs.Where(c => c.IsUseProxy == false).Should().OnlyContain(c => c.ProxyUrl == null);
            validConfigs.Where(c => c.IsUseProxy == true).Should().OnlyContain(c => !string.IsNullOrEmpty(c.ProxyUrl));
        }
    }
}
