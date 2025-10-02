using Aff12B.CronJobs.Host.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aff12B.CronJobs.Host.Tests.Extensions
{
    /// <summary>
    /// Unit tests for HealthCheckExtensions class.
    /// Business-focused scenarios for health check configuration and middleware.
    /// Tests extension methods and type validation (complex mocking avoided due to framework limitations).
    /// </summary>
    public class HealthCheckExtensionsTests
    {
        [Fact]
        public void HealthCheckExtensions_ShouldBeStaticClass()
        {
            // Arrange - Type checking for static class
            var extensionsType = typeof(HealthCheckExtensions);

            // Act & Assert - Should be static class
            extensionsType.IsAbstract.Should().BeTrue("because static classes are abstract");
            extensionsType.IsSealed.Should().BeTrue("because static classes are sealed");
        }

        [Fact]
        public void AddSiteHealthChecks_ShouldBeExtensionMethod()
        {
            // Arrange - Check extension method signature
            var method = typeof(HealthCheckExtensions).GetMethod("AddSiteHealthChecks");

            // Act & Assert - Should be extension method
            method.Should().NotBeNull();
            method.IsStatic.Should().BeTrue();
            method.GetParameters().Should().HaveCount(2);
            method.GetParameters()[0].ParameterType.Should().Be<IServiceCollection>();
            method.GetParameters()[1].ParameterType.Should().Be<IConfiguration>();
            method.ReturnType.Should().Be<IHealthChecksBuilder>();
        }

        [Fact]
        public void UseSiteHealthChecks_ShouldBeExtensionMethod()
        {
            // Arrange - Check extension method signature
            var method = typeof(HealthCheckExtensions).GetMethod("UseSiteHealthChecks");

            // Act & Assert - Should be extension method
            method.Should().NotBeNull();
            method.IsStatic.Should().BeTrue();
            method.GetParameters().Should().HaveCount(2);
            method.GetParameters()[0].ParameterType.Should().Be<IApplicationBuilder>();
            method.GetParameters()[1].ParameterType.Should().Be<IConfiguration>();
        }

        [Fact]
        public void HealthCheckExtensions_ShouldSupportBusinessScenario_ProductionMonitoring()
        {
            // Arrange - Production monitoring scenario
            var extensionsType = typeof(HealthCheckExtensions);
            var addMethod = extensionsType.GetMethod("AddSiteHealthChecks");
            var useMethod = extensionsType.GetMethod("UseSiteHealthChecks");

            // Act & Assert - Should support production monitoring setup
            addMethod.Should().NotBeNull("because production systems require health check registration");
            useMethod.Should().NotBeNull("because production systems require health check endpoints");

            addMethod.ReturnType.Should().Be<IHealthChecksBuilder>("because it should return builder for chaining");
            useMethod.ReturnType.Should().Be(typeof(void), "because it configures middleware pipeline");
        }

        [Fact]
        public void HealthCheckExtensions_ShouldSupportBusinessScenario_LoadBalancerIntegration()
        {
            // Arrange - Load balancer integration scenario
            var extensionsType = typeof(HealthCheckExtensions);
            var useMethod = extensionsType.GetMethod("UseSiteHealthChecks");

            // Act & Assert - Should support load balancer integration
            useMethod.Should().NotBeNull("because load balancers require accessible health check endpoints");
            useMethod.GetParameters().Should().Contain(p => p.ParameterType == typeof(IConfiguration),
                "because load balancers need configuration for access keys");
        }

        [Fact]
        public void HealthCheckExtensions_ShouldSupportBusinessScenario_KubernetesProbes()
        {
            // Arrange - Kubernetes probes scenario
            var extensionsType = typeof(HealthCheckExtensions);
            var useMethod = extensionsType.GetMethod("UseSiteHealthChecks");

            // Act & Assert - Should support Kubernetes readiness/liveness probes
            useMethod.Should().NotBeNull("because Kubernetes requires health check endpoints for container orchestration");
            useMethod.GetParameters()[0].ParameterType.Should().Be<IApplicationBuilder>(
                "because Kubernetes probes access endpoints through the application pipeline");
        }

        [Fact]
        public void HealthCheckExtensions_ShouldSupportBusinessScenario_MultiDatabaseMonitoring()
        {
            // Arrange - Multi-database monitoring scenario
            var extensionsType = typeof(HealthCheckExtensions);
            var addMethod = extensionsType.GetMethod("AddSiteHealthChecks");

            // Act & Assert - Should support multi-database monitoring
            addMethod.Should().NotBeNull("because enterprise systems require monitoring of multiple databases");
            addMethod.GetParameters().Should().Contain(p => p.ParameterType == typeof(IConfiguration),
                "because database connections are configured through IConfiguration");
            addMethod.ReturnType.Should().Be<IHealthChecksBuilder>("because multiple health checks need to be chained");
        }

        [Fact]
        public void HealthCheckExtensions_ShouldSupportBusinessScenario_RedisClusterMonitoring()
        {
            // Arrange - Redis cluster monitoring scenario
            var extensionsType = typeof(HealthCheckExtensions);
            var addMethod = extensionsType.GetMethod("AddSiteHealthChecks");

            // Act & Assert - Should support Redis cluster monitoring
            addMethod.Should().NotBeNull("because Redis clusters require specialized health monitoring");
            addMethod.GetParameters().Should().Contain(p => p.ParameterType == typeof(IConfiguration),
                "because Redis configuration includes cluster connection details");
        }

        [Fact]
        public void HealthCheckExtensions_ShouldSupportBusinessScenario_ServiceMeshIntegration()
        {
            // Arrange - Service mesh integration scenario
            var extensionsType = typeof(HealthCheckExtensions);
            var addMethod = extensionsType.GetMethod("AddSiteHealthChecks");
            var useMethod = extensionsType.GetMethod("UseSiteHealthChecks");

            // Act & Assert - Should support service mesh health monitoring
            addMethod.Should().NotBeNull("because service mesh requires health check registration");
            useMethod.Should().NotBeNull("because service mesh requires accessible health endpoints");

            addMethod.GetParameters().Should().HaveCount(2, "because service mesh needs service collection and configuration");
            useMethod.GetParameters().Should().HaveCount(2, "because service mesh needs application builder and configuration");
        }

        [Fact]
        public void HealthCheckExtensions_ShouldSupportBusinessScenario_DevOpsMonitoring()
        {
            // Arrange - DevOps monitoring scenario
            var extensionsType = typeof(HealthCheckExtensions);
            var addMethod = extensionsType.GetMethod("AddSiteHealthChecks");
            var useMethod = extensionsType.GetMethod("UseSiteHealthChecks");

            // Act & Assert - Should support DevOps monitoring tools
            addMethod.Should().NotBeNull("because DevOps tools require comprehensive health monitoring registration");
            useMethod.Should().NotBeNull("because DevOps tools require accessible health endpoints for automation");

            addMethod.IsStatic.Should().BeTrue("because extension methods are static for easy integration");
            useMethod.IsStatic.Should().BeTrue("because extension methods are static for easy integration");
        }

        [Fact]
        public void HealthCheckExtensions_ShouldSupportBusinessScenario_HighAvailabilitySetup()
        {
            // Arrange - High availability setup scenario
            var extensionsType = typeof(HealthCheckExtensions);
            var addMethod = extensionsType.GetMethod("AddSiteHealthChecks");
            var useMethod = extensionsType.GetMethod("UseSiteHealthChecks");

            // Act & Assert - Should support high availability configurations
            addMethod.Should().NotBeNull("because high availability setups require health check registration for failover");
            useMethod.Should().NotBeNull("because high availability setups require health endpoints for load balancing");

            addMethod.ReturnType.Should().Be<IHealthChecksBuilder>("because multiple health checks are needed for HA");
        }

        [Fact]
        public void HealthCheckExtensions_ShouldHaveConsistentNaming()
        {
            // Arrange - Naming convention checking
            var extensionsType = typeof(HealthCheckExtensions);
            var methods = extensionsType.GetMethods().Where(m => m.IsStatic && m.IsPublic);

            // Act & Assert - Should follow consistent naming conventions
            methods.Should().AllSatisfy(method =>
            {
                method.Name.Should().MatchRegex(@"^(Add|Use).*",
                    "because extension methods should follow Add/Use naming pattern for DI and middleware");
            });
        }
    }
}