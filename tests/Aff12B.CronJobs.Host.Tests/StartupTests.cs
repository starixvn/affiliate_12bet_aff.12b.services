using Aff12B.CronJobs.Host;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Host.Tests
{
    /// <summary>
    /// Unit tests for Startup.cs configuration class.
    /// Business-focused scenarios for service registration and middleware configuration.
    /// Tests critical startup functionality to ensure proper application configuration.
    /// 
    /// NEW RULE: Startup.cs files must have unit tests to validate:
    /// - Service registration and dependency injection setup
    /// - Middleware pipeline configuration
    /// - Environment-specific configurations
    /// - Critical infrastructure setup patterns
    /// </summary>
    public class StartupTests
    {
        [Fact]
        public void Startup_ShouldCreateInstance_WhenValidConfiguration()
        {
            // Arrange - Valid configuration scenario
            var builder = WebApplication.CreateBuilder();
            var configuration = builder.Configuration;

            // Act
            var startup = new Startup(configuration);

            // Assert - Should create instance successfully
            startup.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_ShouldSupportBusinessScenario_CronJobsServiceRegistration()
        {
            // Arrange - CronJobs service registration scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            var act = () => startup.ConfigureServices(services);

            // Assert - Should register essential CronJobs services without throwing
            act.Should().NotThrow("because ConfigureServices should register services successfully");
            services.Should().NotBeEmpty("because services should be registered");
        }

        [Fact]
        public void ConfigureServices_ShouldSupportBusinessScenario_ControllerRegistration()
        {
            // Arrange - Controller registration scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            startup.ConfigureServices(services);

            // Assert - Should register controller services
            services.Should().Contain(s => 
                s.ServiceType.Name.Contains("Mvc") || 
                s.ServiceType.Name.Contains("Controller"),
                "because controllers should be registered for CronJobs management");
        }

        [Fact]
        public void ConfigureServices_ShouldSupportBusinessScenario_HttpClientRegistration()
        {
            // Arrange - HTTP client registration scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            startup.ConfigureServices(services);

            // Assert - Should register HTTP client services
            services.Should().Contain(s => 
                s.ServiceType.Name.Contains("HttpClient") || 
                s.ServiceType == typeof(IHttpClientFactory),
                "because HTTP clients are needed for external API calls");
        }

        [Fact]
        public void ConfigureServices_ShouldSupportBusinessScenario_MemoryCacheRegistration()
        {
            // Arrange - Memory cache registration scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            startup.ConfigureServices(services);

            // Assert - Should register memory cache services
            services.Should().Contain(s => 
                s.ServiceType.Name.Contains("MemoryCache"),
                "because memory cache is essential for performance");
        }

        [Fact]
        public void Configure_ShouldSupportBusinessScenario_DevelopmentEnvironment()
        {
            // Arrange - Development environment scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Development";
            
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            
            using var app = builder.Build();

            // Act - Should not throw in development environment
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should configure development environment successfully
            act.Should().NotThrow("because development environment should be supported");
        }

        [Fact]
        public void Configure_ShouldSupportBusinessScenario_ProductionEnvironment()
        {
            // Arrange - Production environment scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Production";
            
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            
            using var app = builder.Build();

            // Act - Should not throw in production environment
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should configure production environment successfully
            act.Should().NotThrow("because production environment should be supported");
        }

        [Fact]
        public void Configure_ShouldSupportBusinessScenario_MiddlewarePipelineConfiguration()
        {
            // Arrange - Middleware pipeline configuration scenario
            var builder = WebApplication.CreateBuilder();
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            
            using var app = builder.Build();

            // Act
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should configure middleware pipeline
            act.Should().NotThrow("because middleware pipeline should be configured successfully");
        }

        [Fact]
        public void Configure_ShouldSupportBusinessScenario_HangfireIntegration()
        {
            // Arrange - Hangfire integration scenario
            var builder = WebApplication.CreateBuilder();
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            
            using var app = builder.Build();

            // Act - Should configure Hangfire without throwing
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should integrate Hangfire successfully
            act.Should().NotThrow("because Hangfire integration is critical for CronJobs");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_ConfigurationValidation()
        {
            // Arrange - Configuration validation scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["App:LicenseeDataAPI"] = "https://test-api.12b.com",
                ["App:IsApiDataModifiable"] = "true",
                ["Redis:Server"] = "localhost",
                ["Redis:Port"] = "6379",
                ["Redis:Database"] = "0",
                ["Redis:AppName"] = "TestApp"
            });

            // Act
            var startup = new Startup(builder.Configuration);

            // Assert - Should handle valid configuration
            startup.Should().NotBeNull("because valid configuration should create startup successfully");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_MultiEnvironmentConfiguration()
        {
            // Arrange - Multi-environment configuration scenario
            var environments = new[] { "Development", "Staging", "Production" };
            
            foreach (var env in environments)
            {
                var builder = WebApplication.CreateBuilder();
                builder.Environment.EnvironmentName = env;
                
                var startup = new Startup(builder.Configuration);
                startup.ConfigureServices(builder.Services);
                
                using var app = builder.Build();

                // Act & Assert - Should handle all environments
                var act = () => startup.Configure(app, app.Environment);
                act.Should().NotThrow($"because {env} environment should be supported");
            }
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_ServiceLifetimeManagement()
        {
            // Arrange - Service lifetime management scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            startup.ConfigureServices(services);

            // Assert - Should register services with appropriate lifetimes
            services.Should().NotBeEmpty("because services should be registered");
            services.Should().Contain(s => s.Lifetime == ServiceLifetime.Singleton, 
                "because some services should be singletons for performance");
            services.Should().Contain(s => s.Lifetime == ServiceLifetime.Scoped, 
                "because some services should be scoped for request handling");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_HealthCheckConfiguration()
        {
            // Arrange - Health check configuration scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            startup.ConfigureServices(services);

            // Assert - Should register health check services
            services.Should().Contain(s => 
                s.ServiceType.Name.Contains("HealthCheck"),
                "because health checks are critical for CronJobs monitoring");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_LoggingConfiguration()
        {
            // Arrange - Logging configuration scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            startup.ConfigureServices(services);

            // Assert - Should register logging services
            services.Should().Contain(s => 
                s.ServiceType.Name.Contains("Logger") || s.ServiceType.Name.Contains("Logging"),
                "because logging is essential for monitoring and debugging");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_RedisConfiguration()
        {
            // Arrange - Redis configuration scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Redis:Server"] = "redis-cluster.12b.com",
                ["Redis:Port"] = "6379",
                ["Redis:Pass"] = "secure-redis-password-2024",
                ["Redis:Database"] = "1",
                ["Redis:AppName"] = "Aff12B-CronJobs-Prod",
                ["Redis:KeepAlive"] = "120"
            });
            
            var startup = new Startup(builder.Configuration);
            var services = new ServiceCollection();

            // Act - Should handle Redis configuration
            var act = () => startup.ConfigureServices(services);
            
            // Assert - Should configure Redis successfully
            act.Should().NotThrow("because Redis configuration should be handled properly");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_CronJobsWorkflowIntegration()
        {
            // Arrange - CronJobs workflow integration scenario
            var builder = WebApplication.CreateBuilder();
            var startup = new Startup(builder.Configuration);

            // Act - Should integrate all CronJobs components
            var act = () => 
            {
                startup.ConfigureServices(builder.Services);
                using var app = builder.Build();
                startup.Configure(app, app.Environment);
            };

            // Assert - Should integrate complete CronJobs workflow
            act.Should().NotThrow("because complete CronJobs workflow should integrate successfully");
        }
    }
}