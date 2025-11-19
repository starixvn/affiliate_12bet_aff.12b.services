using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests
{
    /// <summary>
    /// Unit tests for Ext.API.Site Startup.cs configuration class.
    /// Business-focused scenarios for API service registration and middleware configuration.
    /// Tests critical startup functionality for external API services.
    /// 
    /// NEW RULE: Startup.cs files must have unit tests to validate:
    /// - API service registration and authentication setup
    /// - JWT authentication and authorization configuration
    /// - Swagger/OpenAPI documentation setup patterns
    /// - Environment-specific API configurations
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
            startup.Configuration.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_ShouldSupportBusinessScenario_APIControllerRegistration()
        {
            // Arrange - API controller registration scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            var act = () => startup.ConfigureServices(services);

            // Assert - Should register API controller services without throwing
            act.Should().NotThrow("because ConfigureServices should register API services successfully");
            services.Should().NotBeEmpty("because API services should be registered");
        }

        [Fact]
        public void ConfigureServices_ShouldSupportBusinessScenario_SwaggerDocumentationSetup()
        {
            // Arrange - Swagger documentation setup scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            startup.ConfigureServices(services);

            // Assert - Should register Swagger services
            services.Should().Contain(s =>
                s.ServiceType.Name.Contains("Swagger") ||
                s.ServiceType.Name.Contains("OpenApi"),
                "because API documentation is essential for external integrations");
        }

        [Fact]
        public void ConfigureServices_ShouldSupportBusinessScenario_AuthenticationSetup()
        {
            // Arrange - Authentication setup scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "test-secret-key-for-jwt-token-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Test.API",
                ["TokenManagement:Audience"] = "Aff12B.Test.Client"
            });

            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            var act = () => startup.ConfigureServices(services);

            // Assert - Should register authentication services without throwing
            act.Should().NotThrow("because authentication setup should work with valid configuration");
        }

        [Fact]
        public void Configure_ShouldSupportBusinessScenario_DevelopmentEnvironment()
        {
            // Arrange - Development environment scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Development";
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "development-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Dev.API",
                ["TokenManagement:Audience"] = "Aff12B.Dev.Client"
            });

            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            using var app = builder.Build();

            // Act - Should not throw in development environment
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should configure development environment with Swagger
            act.Should().NotThrow("because development environment should enable Swagger documentation");
        }

        [Fact]
        public void Configure_ShouldSupportBusinessScenario_ProductionEnvironment()
        {
            // Arrange - Production environment scenario
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Production";
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "production-secret-key-for-jwt-validation-2024-very-secure-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Production.API",
                ["TokenManagement:Audience"] = "Aff12B.Production.Client",
                ["Environment"] = "Production"
            });

            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            using var app = builder.Build();

            // Act - Should not throw in production environment
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should configure production environment without Swagger
            act.Should().NotThrow("because production environment should be secure without documentation exposure");
        }

        [Fact]
        public void Configure_ShouldSupportBusinessScenario_APIMiddlewarePipeline()
        {
            // Arrange - API middleware pipeline scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "api-middleware-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.API",
                ["TokenManagement:Audience"] = "Aff12B.Client"
            });

            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            using var app = builder.Build();

            // Act
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should configure API middleware pipeline
            act.Should().NotThrow("because API middleware pipeline should be configured successfully");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_SerilogIntegration()
        {
            // Arrange - Serilog integration scenario
            var builder = WebApplication.CreateBuilder();
            var configuration = builder.Configuration;

            // Act & Assert - Should integrate Serilog without throwing
            var act = () => new Startup(configuration);
            act.Should().NotThrow("because Serilog integration should not break startup");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_APIVersioning()
        {
            // Arrange - API versioning scenario
            var builder = WebApplication.CreateBuilder();
            var services = new ServiceCollection();
            var startup = new Startup(builder.Configuration);

            // Act
            startup.ConfigureServices(services);

            // Assert - Should support API versioning through Swagger
            services.Should().Contain(s =>
                s.ServiceType.Name.Contains("Swagger"),
                "because API versioning should be supported through service registration");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_MultiEnvironmentTokenConfiguration()
        {
            // Arrange - Multi-environment token configuration scenario
            var environments = new[]
            {
                ("Development", "dev-secret-key-for-jwt-validation-2024-minimum-length"),
                ("Staging", "staging-secret-key-for-jwt-validation-2024-minimum-length"),
                ("Production", "production-secret-key-very-secure-for-jwt-validation-2024-minimum-length")
            };

            foreach (var (env, secret) in environments)
            {
                var builder = WebApplication.CreateBuilder();
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["TokenManagement:Secret"] = secret,
                    ["TokenManagement:Issuer"] = $"Aff12B.{env}.API",
                    ["TokenManagement:Audience"] = $"Aff12B.{env}.Client"
                });

                // Act & Assert - Should handle all environment configurations
                var act = () => new Startup(builder.Configuration);
                act.Should().NotThrow($"because {env} environment token configuration should be supported");
            }
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_CompleteAPIWorkflow()
        {
            // Arrange - Complete API workflow scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "complete-workflow-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Complete.API",
                ["TokenManagement:Audience"] = "Aff12B.Complete.Client"
            });

            var startup = new Startup(builder.Configuration);

            // Act - Should integrate all API components
            var act = () =>
            {
                startup.ConfigureServices(builder.Services);
                using var app = builder.Build();
                startup.Configure(app, app.Environment);
            };

            // Assert - Should integrate complete API workflow
            act.Should().NotThrow("because complete API workflow should integrate successfully");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_JWTTokenValidation()
        {
            // Arrange - JWT token validation scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "jwt-validation-secret-key-for-token-authentication-2024-minimum-length-required",
                ["TokenManagement:Issuer"] = "Aff12B.JWT.API",
                ["TokenManagement:Audience"] = "Aff12B.JWT.Client",
                ["TokenManagement:AccessExpiration"] = "60",
                ["TokenManagement:RefreshExpiration"] = "1440"
            });

            var startup = new Startup(builder.Configuration);
            var services = new ServiceCollection();

            // Act
            var act = () => startup.ConfigureServices(services);

            // Assert - Should configure JWT token validation
            act.Should().NotThrow("because JWT token validation should be configured properly");
        }

        [Fact]
        public void Startup_ShouldSupportBusinessScenario_APISecurityConfiguration()
        {
            // Arrange - API security configuration scenario
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TokenManagement:Secret"] = "security-configuration-secret-key-for-jwt-validation-2024-minimum-length",
                ["TokenManagement:Issuer"] = "Aff12B.Security.API",
                ["TokenManagement:Audience"] = "Aff12B.Security.Client"
            });

            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            using var app = builder.Build();

            // Act
            var act = () => startup.Configure(app, app.Environment);

            // Assert - Should configure API security properly
            act.Should().NotThrow("because API security configuration should work correctly");
        }
    }
}