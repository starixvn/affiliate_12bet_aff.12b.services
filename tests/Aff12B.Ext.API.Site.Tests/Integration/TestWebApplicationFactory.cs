using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aff12B.Ext.API.Site.Tests.Integration
{
    /// <summary>
    /// Custom WebApplicationFactory for integration testing with test configurations
    /// </summary>
    public class TestWebApplicationFactory : WebApplicationFactory<Aff12B.Ext.API.Site.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Clear existing configuration
                config.Sources.Clear();

                // Add test configuration
                config.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", "Data Source=:memory:"),
                    new KeyValuePair<string, string>("TokenManagement:Secret", "test-secret-key-for-jwt-token-generation-minimum-256-bits"),
                    new KeyValuePair<string, string>("TokenManagement:Issuer", "TestIssuer"),
                    new KeyValuePair<string, string>("TokenManagement:Audience", "TestAudience"),
                    new KeyValuePair<string, string>("TokenManagement:AccessExpiration", "60"),
                    new KeyValuePair<string, string>("Users:0:UserName", "testuser"),
                    new KeyValuePair<string, string>("Users:0:Password", "5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8"),
                    new KeyValuePair<string, string>("Users:1:UserName", "admin"),
                    new KeyValuePair<string, string>("Users:1:Password", "EF92B778BAFE771E89245B89ECBC08A44A4E166C06659911881F383D4473E94F")
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove existing registrations for testing
                var descriptors = services.Where(d =>
                    d.ServiceType.Name.Contains("Repository") ||
                    d.ServiceType.Name.Contains("Service"))
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Add test services
                services.AddSingleton<TokenManagement>(provider =>
                {
                    var config = provider.GetRequiredService<IConfiguration>();
                    return new TokenManagement
                    {
                        Secret = config["TokenManagement:Secret"],
                        Issuer = config["TokenManagement:Issuer"],
                        Audience = config["TokenManagement:Audience"],
                        AccessExpiration = int.Parse(config["TokenManagement:AccessExpiration"] ?? "60")
                    };
                });

                services.AddSingleton<List<User>>(provider =>
                {
                    return new List<User>
                    {
                        new User
                        {
                            UserName = "testuser",
                            Password = "5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8"
                        },
                        new User
                        {
                            UserName = "admin",
                            Password = "EF92B778BAFE771E89245B89ECBC08A44A4E166C06659911881F383D4473E94F"
                        }
                    };
                });

                // Configure logging for tests - disabled for performance
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.None);
                });
            });

            builder.UseEnvironment("Test");
        }

        /// <summary>
        /// Creates a factory with custom service overrides
        /// </summary>
        /// <param name="serviceOverrides">Dictionary of service type to mock instance</param>
        /// <returns>Configured factory</returns>
        public static TestWebApplicationFactory CreateWithMocks(Dictionary<Type, object> serviceOverrides = null)
        {
            var factory = new TestWebApplicationFactory();

            if (serviceOverrides != null)
            {
                return (TestWebApplicationFactory)factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        foreach (var serviceOverride in serviceOverrides)
                        {
                            // Remove existing service
                            var descriptor = services.FirstOrDefault(d => d.ServiceType == serviceOverride.Key);
                            if (descriptor != null)
                            {
                                services.Remove(descriptor);
                            }

                            // Add mock service
                            services.AddSingleton(serviceOverride.Key, serviceOverride.Value);
                        }
                    });
                });
            }

            return factory;
        }

        /// <summary>
        /// Creates a factory with specific repository mocks
        /// </summary>
        /// <param name="repositoryMocks">Dictionary of repository interfaces to mock instances</param>
        /// <returns>Configured factory</returns>
        public static TestWebApplicationFactory CreateWithRepositoryMocks(Dictionary<Type, Mock> repositoryMocks)
        {
            var serviceOverrides = repositoryMocks.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Object
            );

            return CreateWithMocks(serviceOverrides);
        }
    }
}
