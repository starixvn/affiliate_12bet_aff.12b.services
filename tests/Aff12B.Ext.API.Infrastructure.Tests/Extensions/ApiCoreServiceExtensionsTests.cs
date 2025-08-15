using Aff12B.Ext.API.Core.Services;
using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using Aff12B.Ext.API.Infastructure.Extentions;
using Aff12B.Ext.API.Infastructure.Services;
using Fanex.Data.Repository;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Extensions
{
    public class ApiCoreServiceExtensionsTests
    {
        private readonly IServiceCollection _services;

        public ApiCoreServiceExtensionsTests()
        {
            _services = new ServiceCollection();
        }

        #region AddApiCoreService Tests

        [Fact]
        public void AddApiCoreService_ShouldRegisterIDynamicRepository()
        {
            // Act
            var result = _services.AddApiCoreService();

            // Assert
            var serviceProvider = result.BuildServiceProvider();
            var dynamicRepository = serviceProvider.GetService<IDynamicRepository>();

            dynamicRepository.Should().NotBeNull();
            dynamicRepository.Should().BeOfType<DynamicRepository>();
        }

        [Fact]
        public void AddApiCoreService_ShouldRegisterIAuthServiceAsScoped()
        {
            // Act
            var result = _services.AddApiCoreService();

            // Assert
            var serviceDescriptor = result.FirstOrDefault(s => s.ServiceType == typeof(IAuthService));
            serviceDescriptor.Should().NotBeNull();
            serviceDescriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
            serviceDescriptor.ImplementationType.Should().Be(typeof(AuthService));
        }

        [Fact]
        public void AddApiCoreService_ShouldRegisterIUserManagementServiceAsScoped()
        {
            // Act
            var result = _services.AddApiCoreService();

            // Assert
            var serviceDescriptor = result.FirstOrDefault(s => s.ServiceType == typeof(IUserManagementService));
            serviceDescriptor.Should().NotBeNull();
            serviceDescriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
            serviceDescriptor.ImplementationType.Should().Be(typeof(UserManagementService));
        }

        [Fact]
        public void AddApiCoreService_ShouldReturnServiceCollection()
        {
            // Act
            var result = _services.AddApiCoreService();

            // Assert
            result.Should().BeSameAs(_services);
            result.Should().BeOfType<ServiceCollection>();
        }

        [Fact]
        public void AddApiCoreService_ShouldRegisterDynamicRepositoryAsSingleton()
        {
            // Act
            var result = _services.AddApiCoreService();

            // Assert
            var serviceDescriptor = result.FirstOrDefault(s => s.ServiceType == typeof(IDynamicRepository));
            serviceDescriptor.Should().NotBeNull();
            serviceDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ImplementationType.Should().Be(typeof(DynamicRepository));
        }

        [Fact]
        public void AddApiCoreService_CalledMultipleTimes_ShouldRegisterServicesMultipleTimes()
        {
            // Act
            _services.AddApiCoreService();
            _services.AddApiCoreService();

            // Assert
            var authServiceDescriptors = _services.Where(s => s.ServiceType == typeof(IAuthService)).ToList();
            var userServiceDescriptors = _services.Where(s => s.ServiceType == typeof(IUserManagementService)).ToList();
            var dynamicRepoDescriptors = _services.Where(s => s.ServiceType == typeof(IDynamicRepository)).ToList();

            // Each service should be registered twice
            authServiceDescriptors.Should().HaveCount(2);
            userServiceDescriptors.Should().HaveCount(2);
            dynamicRepoDescriptors.Should().HaveCount(2);
        }

        [Fact]
        public void AddApiCoreService_EmptyServiceCollection_ShouldAddThreeServices()
        {
            // Arrange
            _services.Should().BeEmpty();

            // Act
            var result = _services.AddApiCoreService();

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public void AddApiCoreService_ExistingServices_ShouldAppendNewServices()
        {
            // Arrange
            _services.AddTransient<string>(_ => "test");
            _services.Should().HaveCount(1);

            // Act
            var result = _services.AddApiCoreService();

            // Assert
            result.Should().HaveCount(4); // 1 existing + 3 new
        }

        #endregion

        #region Service Resolution Tests

        [Fact]
        public void AddApiCoreService_ServiceProvider_ShouldResolveAllServices()
        {
            // Arrange
            // Add required dependencies for UserManagementService and AuthService
            _services.Configure<List<User>>(users => { });
            _services.Configure<TokenManagement>(token =>
            {
                token.Secret = "super-secret-key-that-is-long-enough-for-hmac-sha256-algorithm-requirements";
                token.AccessExpiration = 60;
            });
            _services.AddApiCoreService();
            var serviceProvider = _services.BuildServiceProvider();

            // Act & Assert
            var dynamicRepository = serviceProvider.GetService<IDynamicRepository>();
            var authService = serviceProvider.GetService<IAuthService>();
            var userService = serviceProvider.GetService<IUserManagementService>();

            dynamicRepository.Should().NotBeNull();
            authService.Should().NotBeNull();
            userService.Should().NotBeNull();
        }

        [Fact]
        public void AddApiCoreService_ServiceProvider_ShouldRespectLifetimes()
        {
            // Arrange
            // Add required dependencies
            _services.Configure<List<User>>(users => { });
            _services.Configure<TokenManagement>(token =>
            {
                token.Secret = "super-secret-key-that-is-long-enough-for-hmac-sha256-algorithm-requirements";
                token.AccessExpiration = 60;
            });
            _services.AddApiCoreService();
            var serviceProvider = _services.BuildServiceProvider();

            // Act
            var dynamicRepo1 = serviceProvider.GetService<IDynamicRepository>();
            var dynamicRepo2 = serviceProvider.GetService<IDynamicRepository>();

            using var scope1 = serviceProvider.CreateScope();
            using var scope2 = serviceProvider.CreateScope();

            var authService1 = scope1.ServiceProvider.GetService<IAuthService>();
            var authService2 = scope1.ServiceProvider.GetService<IAuthService>();
            var authService3 = scope2.ServiceProvider.GetService<IAuthService>();

            // Assert
            // Singleton: same instance across the entire application
            dynamicRepo1.Should().BeSameAs(dynamicRepo2);

            // Scoped: same instance within a scope, different across scopes
            authService1.Should().BeSameAs(authService2);
            authService1.Should().NotBeSameAs(authService3);
        }

        [Fact]
        public void AddApiCoreService_WithExistingIDynamicRepository_ShouldRegisterBoth()
        {
            // Arrange
            _services.AddSingleton<IDynamicRepository>(_ => new Mock<IDynamicRepository>().Object);

            // Act
            _services.AddApiCoreService();
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var repositories = serviceProvider.GetServices<IDynamicRepository>().ToList();
            repositories.Should().HaveCount(2);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void AddApiCoreService_RealWorldUsage_ShouldWorkWithAspNetCore()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            // Add required dependencies
            services.Configure<List<User>>(users => { });
            services.Configure<TokenManagement>(token =>
            {
                token.Secret = "super-secret-key-that-is-long-enough-for-hmac-sha256-algorithm-requirements";
                token.AccessExpiration = 60;
            });

            // Act
            services.AddApiCoreService();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var dynamicRepository = serviceProvider.GetRequiredService<IDynamicRepository>();
            var authService = serviceProvider.GetRequiredService<IAuthService>();
            var userService = serviceProvider.GetRequiredService<IUserManagementService>();

            dynamicRepository.Should().NotBeNull();
            authService.Should().NotBeNull();
            userService.Should().NotBeNull();

            // Verify that services are properly constructed
            dynamicRepository.Should().BeOfType<DynamicRepository>();
            authService.Should().BeOfType<AuthService>();
            userService.Should().BeOfType<UserManagementService>();
        }

        #endregion
    }
}
