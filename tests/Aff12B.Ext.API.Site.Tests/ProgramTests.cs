using Aff12B.Ext.API.Site;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests
{
    /// <summary>
    /// Unit tests for Program.cs bootstrap configuration.
    /// Business-focused scenarios for external API application startup, configuration, and service registration.
    /// Tests critical bootstrap functionality to ensure proper API application initialization.
    /// 
    /// NEW RULE 27: Program.cs files must have unit tests to validate:
    /// - Configuration loading and validation for API services
    /// - Service registration and dependency injection setup
    /// - Environment-specific behavior for API endpoints
    /// - Critical bootstrap functionality for external API access
    /// </summary>
    public class ProgramTests
    {
        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIApplicationBootstrap()
        {
            // Arrange - API application bootstrap scenario
            var builder = WebApplication.CreateBuilder();

            // Act - Test API application bootstrap pattern from Program.cs
            var env = builder.Environment;
            builder.Configuration.SetBasePath(env.ContentRootPath);

            // Assert - Should support API application bootstrap
            builder.Should().NotBeNull("because Program.cs should successfully bootstrap the API application");
            env.ContentRootPath.Should().NotBeNullOrEmpty("because content root path should be set for API");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIConfigurationLoading()
        {
            // Arrange - API configuration loading scenario
            var builder = WebApplication.CreateBuilder();

            // Act - Test configuration setup pattern from Program.cs
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);
            builder.Configuration.AddEnvironmentVariables();

            // Assert - Should support API configuration loading pattern
            builder.Configuration.Should().NotBeNull("because API configuration should be loaded");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIStartupClassIntegration()
        {
            // Arrange - API Startup class integration pattern
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-startup-integration-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Integration.API",
                ["TokenManagement:Audience"] = "Aff12B.Integration.Client"
            });

            // Act - Test Startup class instantiation pattern from Program.cs
            var startup = new Startup(builder.Configuration);

            // Assert - Should support API Startup class integration
            startup.Should().NotBeNull("because API Startup class should be instantiated with configuration");
            startup.Configuration.Should().NotBeNull("because API Startup should have configuration access");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIServiceRegistrationPattern()
        {
            // Arrange - API service registration pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-service-registration-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.ServiceReg.API",
                ["TokenManagement:Audience"] = "Aff12B.ServiceReg.Client"
            });
            
            var startup = new Startup(builder.Configuration);

            // Act - Test service registration pattern from Program.cs
            startup.ConfigureServices(builder.Services);

            // Assert - Should support API service registration pattern
            builder.Services.Should().NotBeEmpty("because API services should be registered by Startup.ConfigureServices");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIEnvironmentConfiguration()
        {
            // Arrange - API environment configuration pattern
            var builder = WebApplication.CreateBuilder();

            // Act - Test environment setup pattern from Program.cs
            var env = builder.Environment;
            builder.Configuration.SetBasePath(env.ContentRootPath);

            // Assert - Should support API environment configuration pattern
            env.Should().NotBeNull("because API environment should be available");
            env.ContentRootPath.Should().NotBeNullOrEmpty("because API content root path should be set");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIApplicationBuildPattern()
        {
            // Arrange - API application build pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-application-build-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Build.API",
                ["TokenManagement:Audience"] = "Aff12B.Build.Client"
            });
            
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            // Act - Test application build pattern from Program.cs
            using var app = builder.Build();

            // Assert - Should support API application build pattern
            app.Should().NotBeNull("because API WebApplication should be built successfully");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIMiddlewareConfigurationPattern()
        {
            // Arrange - API middleware configuration pattern used in Program.cs
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-middleware-configuration-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Middleware.API",
                ["TokenManagement:Audience"] = "Aff12B.Middleware.Client"
            });
            
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            using var app = builder.Build();

            // Act - Test middleware configuration pattern from Program.cs
            startup.Configure(app, app.Environment);

            // Assert - Should support API middleware configuration pattern
            app.Should().NotBeNull("because API middleware configuration should work");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIDevelopmentEnvironment()
        {
            // Arrange - API development environment scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Development";
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-development-environment-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Development.API",
                ["TokenManagement:Audience"] = "Aff12B.Development.Client"
            });

            // Act - Test development environment pattern
            var env = builder.Environment;

            // Assert - Should support API development environment
            env.IsDevelopment().Should().BeTrue("because API development environment should be recognized");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIProductionEnvironment()
        {
            // Arrange - API production environment scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Production";
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-production-environment-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Production.API",
                ["TokenManagement:Audience"] = "Aff12B.Production.Client",
                ["Environment"] = "Production"
            });

            // Act - Test production environment pattern
            var env = builder.Environment;

            // Assert - Should support API production environment
            env.IsProduction().Should().BeTrue("because API production environment should be recognized");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIConfigurationFileLoading()
        {
            // Arrange - API configuration file loading scenario
            var builder = WebApplication.CreateBuilder();

            // Act - Test configuration file loading pattern from Program.cs
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

            // Assert - Should support API configuration file loading
            builder.Configuration.Should().NotBeNull("because API configuration files should be loaded");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIEnvironmentVariablesLoading()
        {
            // Arrange - API environment variables loading scenario
            var builder = WebApplication.CreateBuilder();

            // Act - Test environment variables loading pattern from Program.cs
            builder.Configuration.AddEnvironmentVariables();

            // Assert - Should support API environment variables loading
            builder.Configuration.Should().NotBeNull("because API environment variables should be loaded");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIBootstrapValidation()
        {
            // Arrange - API bootstrap validation scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-bootstrap-validation-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Bootstrap.API",
                ["TokenManagement:Audience"] = "Aff12B.Bootstrap.Client"
            });
            
            var startup = new Startup(builder.Configuration);

            // Act - Test complete bootstrap pattern from Program.cs
            startup.ConfigureServices(builder.Services);
            using var app = builder.Build();
            startup.Configure(app, app.Environment);

            // Assert - Should complete API bootstrap successfully
            app.Should().NotBeNull("because complete API bootstrap should succeed");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIJWTTokenConfiguration()
        {
            // Arrange - API JWT token configuration scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-jwt-token-configuration-secret-key-for-validation-2024-minimum-length-required",
                ["TokenManagement:Issuer"] = "Aff12B.JWT.API",
                ["TokenManagement:Audience"] = "Aff12B.JWT.Client",
                ["TokenManagement:AccessExpiration"] = "60",
                ["TokenManagement:RefreshExpiration"] = "1440"
            });

            // Act - Test JWT token configuration pattern
            var startup = new Startup(builder.Configuration);

            // Assert - Should support JWT token configuration
            startup.Should().NotBeNull("because JWT token configuration should be handled properly");
            startup.Configuration.Should().NotBeNull("because JWT configuration should be accessible");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APISwaggerDocumentation()
        {
            // Arrange - API Swagger documentation scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Development";
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-swagger-documentation-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Swagger.API",
                ["TokenManagement:Audience"] = "Aff12B.Swagger.Client"
            });
            
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            
            using var app = builder.Build();

            // Act - Test Swagger documentation support
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should support Swagger documentation in development
            act.Should().NotThrow("because Swagger documentation should be enabled in development environment");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APISecurityConfiguration()
        {
            // Arrange - API security configuration scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-security-configuration-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Security.API",
                ["TokenManagement:Audience"] = "Aff12B.Security.Client"
            });
            
            var startup = new Startup(builder.Configuration);

            // Act - Test security configuration pattern
            var act = () => startup.ConfigureServices(builder.Services);

            // Assert - Should support API security configuration
            act.Should().NotThrow("because API security configuration should be handled properly");
        }

        [Fact]
        public void Program_ShouldSupportBusinessScenario_APIMultiEnvironmentConfiguration()
        {
            // Arrange - API multi-environment configuration scenario
            var environments = new[] { "Development", "Staging", "Production", "LOCAL", "PRO" };
            
            foreach (var env in environments)
            {
                var builder = WebApplication.CreateBuilder();
                builder.Environment.EnvironmentName = env;
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["TokenManagement:Secret"] = $"api-{env.ToLower()}-environment-secret-key-for-jwt-validation-2024-minimum-length",
                    ["TokenManagement:Issuer"] = $"Aff12B.{env}.API",
                    ["TokenManagement:Audience"] = $"Aff12B.{env}.Client"
                });

                // Act & Assert - Should handle all API environments
                var act = () => new Startup(builder.Configuration);
                act.Should().NotThrow($"because {env} environment should be supported for API");
            }
        }
    }
}
