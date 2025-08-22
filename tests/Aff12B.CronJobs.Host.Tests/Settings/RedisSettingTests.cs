using Aff12B.CronJobs.Host.Settings;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Host.Tests.Settings
{
    /// <summary>
    /// Unit tests for RedisSetting class.
    /// Business-focused scenarios for Redis cache configuration.
    /// Tests configuration scenarios, connection parameters, and business use cases.
    /// </summary>
    public class RedisSettingTests
    {
        [Fact]
        public void RedisSetting_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var redisSetting = new RedisSetting();

            // Assert - Should have default values
            redisSetting.Server.Should().BeNull();
            redisSetting.Port.Should().Be(0);
            redisSetting.Pass.Should().BeNull();
            redisSetting.Database.Should().Be(0);
            redisSetting.AppName.Should().BeNull();
            redisSetting.KeepAlive.Should().Be(0);
        }

        [Fact]
        public void RedisSetting_ShouldSupportBusinessScenario_ProductionRedisCluster()
        {
            // Arrange - Production Redis cluster scenario
            var redisSetting = new RedisSetting
            {
                Server = "redis-cluster-prod.12b.com",
                Port = 6379,
                Pass = "prod-redis-secure-password-2024",
                Database = 0,
                AppName = "Aff12B-CronJobs-Prod",
                KeepAlive = 60
            };

            // Act & Assert - Should support production Redis cluster configuration
            redisSetting.Server.Should().Contain("prod");
            redisSetting.Port.Should().Be(6379, "because 6379 is the standard Redis port");
            redisSetting.Pass.Should().NotBeNullOrEmpty("because production requires authentication");
            redisSetting.Database.Should().Be(0, "because database 0 is typically used for production");
            redisSetting.AppName.Should().StartWith("Aff12B");
            redisSetting.KeepAlive.Should().BeGreaterThan(0, "because production connections need keep-alive");
        }

        [Fact]
        public void RedisSetting_ShouldSupportBusinessScenario_DevelopmentRedis()
        {
            // Arrange - Development Redis scenario
            var redisSetting = new RedisSetting
            {
                Server = "localhost",
                Port = 6379,
                Pass = "dev-password",
                Database = 1,
                AppName = "Aff12B-CronJobs-Dev",
                KeepAlive = 30
            };

            // Act & Assert - Should support development Redis configuration
            redisSetting.Server.Should().Be("localhost");
            redisSetting.Port.Should().Be(6379);
            redisSetting.Pass.Should().Be("dev-password");
            redisSetting.Database.Should().Be(1, "because dev often uses different database number");
            redisSetting.AppName.Should().Contain("Dev");
            redisSetting.KeepAlive.Should().BeLessThan(60, "because dev connections can have shorter keep-alive");
        }

        [Fact]
        public void RedisSetting_ShouldSupportBusinessScenario_StagingRedis()
        {
            // Arrange - Staging Redis scenario
            var redisSetting = new RedisSetting
            {
                Server = "redis-staging.12b.com",
                Port = 6380,
                Pass = "staging-redis-key-2024",
                Database = 2,
                AppName = "Aff12B-CronJobs-Staging",
                KeepAlive = 45
            };

            // Act & Assert - Should support staging Redis configuration
            redisSetting.Server.Should().Contain("staging");
            redisSetting.Port.Should().Be(6380, "because staging might use different port");
            redisSetting.Pass.Should().Contain("staging");
            redisSetting.Database.Should().Be(2);
            redisSetting.AppName.Should().Contain("Staging");
            redisSetting.KeepAlive.Should().BeInRange(30, 60);
        }

        [Fact]
        public void RedisSetting_ShouldSupportBusinessScenario_HangfireJobStorage()
        {
            // Arrange - Hangfire job storage scenario
            var redisSetting = new RedisSetting
            {
                Server = "redis-jobs.12b.com",
                Port = 6379,
                Pass = "hangfire-redis-storage-2024",
                Database = 3,
                AppName = "Hangfire-JobStorage",
                KeepAlive = 120
            };

            // Act & Assert - Should support Hangfire job storage configuration
            redisSetting.Server.Should().Contain("jobs");
            redisSetting.Database.Should().Be(3, "because Hangfire often uses dedicated database");
            redisSetting.AppName.Should().Contain("Hangfire");
            redisSetting.KeepAlive.Should().BeGreaterThan(60, "because job storage needs longer keep-alive");
        }

        [Fact]
        public void RedisSetting_ShouldSupportBusinessScenario_CacheStorage()
        {
            // Arrange - Cache storage scenario
            var redisSetting = new RedisSetting
            {
                Server = "redis-cache.12b.com",
                Port = 6379,
                Pass = "cache-redis-key-2024",
                Database = 4,
                AppName = "Aff12B-Cache",
                KeepAlive = 90
            };

            // Act & Assert - Should support cache storage configuration
            redisSetting.Server.Should().Contain("cache");
            redisSetting.Database.Should().Be(4, "because cache uses dedicated database");
            redisSetting.AppName.Should().Contain("Cache");
            redisSetting.KeepAlive.Should().BeGreaterThan(60, "because cache connections need stability");
        }

        [Theory]
        [InlineData("redis-cluster-1.aws.com", 6379, 0, 60)]
        [InlineData("redis-cluster-2.aws.com", 6380, 1, 90)]
        [InlineData("redis-cluster-3.aws.com", 6381, 2, 120)]
        public void RedisSetting_ShouldSupportVariousClusterConfigurations(string server, int port, int database, int keepAlive)
        {
            // Arrange & Act
            var redisSetting = new RedisSetting
            {
                Server = server,
                Port = port,
                Pass = "cluster-password",
                Database = database,
                AppName = "Aff12B-Cluster",
                KeepAlive = keepAlive
            };

            // Assert - Should accept various cluster configurations
            redisSetting.Server.Should().Be(server);
            redisSetting.Port.Should().Be(port);
            redisSetting.Database.Should().Be(database);
            redisSetting.KeepAlive.Should().Be(keepAlive);
        }

        [Fact]
        public void RedisSetting_ShouldSupportPropertyModification()
        {
            // Arrange
            var redisSetting = new RedisSetting
            {
                Server = "initial-redis.com",
                Port = 6379,
                Pass = "initial-password",
                Database = 0,
                AppName = "Initial-App",
                KeepAlive = 30
            };

            // Act - Modify properties
            redisSetting.Server = "updated-redis.com";
            redisSetting.Port = 6380;
            redisSetting.Pass = "updated-password";
            redisSetting.Database = 5;
            redisSetting.AppName = "Updated-App";
            redisSetting.KeepAlive = 120;

            // Assert - Should support property modification
            redisSetting.Server.Should().Be("updated-redis.com");
            redisSetting.Port.Should().Be(6380);
            redisSetting.Pass.Should().Be("updated-password");
            redisSetting.Database.Should().Be(5);
            redisSetting.AppName.Should().Be("Updated-App");
            redisSetting.KeepAlive.Should().Be(120);
        }

        [Fact]
        public void RedisSetting_ShouldSupportBusinessScenario_HighAvailabilitySetup()
        {
            // Arrange - High availability setup scenario
            var primaryRedisSetting = new RedisSetting
            {
                Server = "redis-primary-ha.12b.com",
                Port = 6379,
                Pass = "ha-redis-primary-2024",
                Database = 0,
                AppName = "Aff12B-HA-Primary",
                KeepAlive = 180
            };

            var secondaryRedisSetting = new RedisSetting
            {
                Server = "redis-secondary-ha.12b.com",
                Port = 6379,
                Pass = "ha-redis-secondary-2024",
                Database = 0,
                AppName = "Aff12B-HA-Secondary",
                KeepAlive = 180
            };

            // Act & Assert - Should support high availability configuration
            primaryRedisSetting.Server.Should().Contain("primary");
            secondaryRedisSetting.Server.Should().Contain("secondary");
            
            primaryRedisSetting.KeepAlive.Should().Be(secondaryRedisSetting.KeepAlive, 
                "because HA setups should have consistent keep-alive settings");
            
            primaryRedisSetting.Database.Should().Be(secondaryRedisSetting.Database,
                "because HA setups should use same database");
        }

        [Fact]
        public void RedisSetting_ShouldHandleNullValues()
        {
            // Arrange & Act
            var redisSetting = new RedisSetting
            {
                Server = null,
                Pass = null,
                AppName = null
            };

            // Assert - Should handle null values gracefully
            redisSetting.Server.Should().BeNull();
            redisSetting.Pass.Should().BeNull();
            redisSetting.AppName.Should().BeNull();
            redisSetting.Port.Should().Be(0);
            redisSetting.Database.Should().Be(0);
            redisSetting.KeepAlive.Should().Be(0);
        }

        [Fact]
        public void RedisSetting_ShouldSupportBusinessScenario_MultiTenantRedis()
        {
            // Arrange - Multi-tenant Redis scenario
            var tenantConfigs = new[]
            {
                new RedisSetting { Server = "redis.tenant-a.com", Database = 0, AppName = "Aff12B-TenantA" },
                new RedisSetting { Server = "redis.tenant-b.com", Database = 0, AppName = "Aff12B-TenantB" },
                new RedisSetting { Server = "redis.tenant-c.com", Database = 0, AppName = "Aff12B-TenantC" }
            };

            // Act & Assert - Should support multi-tenant configuration
            tenantConfigs.Should().HaveCount(3);
            tenantConfigs.Should().AllSatisfy(config =>
            {
                config.Server.Should().Contain("tenant");
                config.AppName.Should().StartWith("Aff12B-Tenant");
            });
        }
    }
}
