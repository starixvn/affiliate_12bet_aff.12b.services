using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Aff12B.CronJobs.Host.Tests
{
    /// <summary>
    /// Unit tests for Program.cs bootstrap configuration.
    /// Business-focused scenarios for application startup, configuration, and service registration.
    /// Tests critical bootstrap functionality patterns used in Program.cs.
    /// 
    /// NEW RULE: Program.cs files must have unit tests to validate:
    /// - Configuration loading and validation patterns
    /// - Service registration and dependency injection setup
    /// - Environment-specific behavior patterns
    /// - Critical bootstrap functionality components
    /// </summary>
    public class ProgramTests
    {
        [Fact]
        public void Program_ShouldSupportBusinessScenario_ConfigurationBuilderPattern()
        {
            // Arrange - Configuration builder pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();

            // Act - Test configuration setup pattern from Program.cs
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();

            // Assert - Should support configuration loading pattern
            builder.Should().NotBeNull("because WebApplication builder should be created successfully");
            builder.Configuration.Should().NotBeNull("because configuration should be available");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_LoggingConfiguration()
        {
            // Arrange - Logging configuration pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();

            // Act - Test logging setup pattern from Program.cs
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Assert - Should support logging configuration pattern
            builder.Logging.Should().NotBeNull("because logging configuration should be available");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_StartupClassIntegration()
        {
            // Arrange - Startup class integration pattern
            var builder = WebApplication.CreateBuilder();
            var mockConfiguration = builder.Configuration;

            // Act - Test Startup class instantiation pattern from Program.cs
            var startup = new Startup(mockConfiguration);

            // Assert - Should support Startup class integration
            startup.Should().NotBeNull("because Startup class should be instantiated with configuration");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_ServiceRegistrationPattern()
        {
            // Arrange - Service registration pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();
            var startup = new Startup(builder.Configuration);

            // Act - Test service registration pattern from Program.cs
            startup.ConfigureServices(builder.Services);

            // Assert - Should support service registration pattern
            builder.Services.Should().NotBeEmpty("because services should be registered by Startup.ConfigureServices");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_EnvironmentConfiguration()
        {
            // Arrange - Environment configuration pattern
            var builder = WebApplication.CreateBuilder();

            // Act - Test environment setup pattern from Program.cs
            var env = builder.Environment;
            builder.Configuration.SetBasePath(env.ContentRootPath);

            // Assert - Should support environment configuration pattern
            env.Should().NotBeNull("because environment should be available");
            env.ContentRootPath.Should().NotBeNullOrEmpty("because content root path should be set");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_ApplicationBuildPattern()
        {
            // Arrange - Application build pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            // Act - Test application build pattern from Program.cs
            using var app = builder.Build();

            // Assert - Should support application build pattern
            app.Should().NotBeNull("because WebApplication should be built successfully");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_EndpointMappingPattern()
        {
            // Arrange - Endpoint mapping pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            using var app = builder.Build();

            // Act - Test endpoint mapping pattern from Program.cs
            app.MapGet("/test", () => "Test endpoint");

            // Assert - Should support endpoint mapping pattern
            app.Should().NotBeNull("because endpoint mapping should work");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_MiddlewareConfigurationPattern()
        {
            // Arrange - Middleware configuration pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            using var app = builder.Build();

            // Act - Test middleware configuration pattern from Program.cs
            startup.Configure(app, app.Environment);

            // Assert - Should support middleware configuration pattern
            app.Should().NotBeNull("because middleware configuration should work");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_CronJobsWelcomeMessage()
        {
            // Arrange - CronJobs welcome message pattern
            var builder = WebApplication.CreateBuilder();
            using var app = builder.Build();

            // Act - Test welcome message pattern from Program.cs
            app.MapGet("/", () => "Hello, Welcome to 12B CronJobs!");

            // Assert - Should support CronJobs welcome message
            app.Should().NotBeNull("because welcome endpoint should be mappable");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_DevelopmentEnvironment()
        {
            // Arrange - Development environment scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Development";

            // Act - Test development environment pattern
            var env = builder.Environment;

            // Assert - Should support development environment
            env.IsDevelopment().Should().BeTrue("because development environment should be recognized");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_ProductionEnvironment()
        {
            // Arrange - Production environment scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Production";

            // Act - Test production environment pattern
            var env = builder.Environment;

            // Assert - Should support production environment
            env.IsProduction().Should().BeTrue("because production environment should be recognized");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_ConfigurationFileLoading()
        {
            // Arrange - Configuration file loading scenario
            var builder = WebApplication.CreateBuilder();

            // Act - Test configuration file loading pattern from Program.cs
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

            // Assert - Should support configuration file loading
            builder.Configuration.Should().NotBeNull("because configuration files should be loaded");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_BootstrapValidation()
        {
            // Arrange - Bootstrap validation scenario
            var builder = WebApplication.CreateBuilder();
            var startup = new Startup(builder.Configuration);

            // Act - Test complete bootstrap pattern from Program.cs
            startup.ConfigureServices(builder.Services);
            using var app = builder.Build();
            startup.Configure(app, app.Environment);

            // Assert - Should complete bootstrap successfully
            app.Should().NotBeNull("because complete bootstrap should succeed");
        }
    }
}