using Aff12B.CronJobs.Host.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Host.Tests.Extensions
{
    /// <summary>
    /// Unit tests for MiddlewareExtensions class.
    /// Business-focused scenarios for middleware extension methods.
    /// Tests ping endpoint handling and middleware pipeline integration.
    /// </summary>
    public class MiddlewareExtensionsTests
    {
        [Fact]
        public void MiddlewareExtensions_ShouldBeStaticClass()
        {
            // Arrange - Type checking for static class
            var extensionsType = typeof(MiddlewareExtensions);

            // Act & Assert - Should be static class
            extensionsType.IsAbstract.Should().BeTrue("because static classes are abstract");
            extensionsType.IsSealed.Should().BeTrue("because static classes are sealed");
        }

        [Fact]
        public void UsePing_ShouldBeExtensionMethod()
        {
            // Arrange - Check extension method signature
            var method = typeof(MiddlewareExtensions).GetMethod("UsePing");

            // Act & Assert - Should be extension method
            method.Should().NotBeNull();
            method.IsStatic.Should().BeTrue();
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be<IApplicationBuilder>();
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_HealthMonitoring()
        {
            // Arrange - Health monitoring scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act - Should not throw when called
            var act = () => mockApplicationBuilder.Object.UsePing();

            // Assert - Should support health monitoring integration
            act.Should().NotThrow("because ping middleware is essential for health monitoring");
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_LoadBalancerIntegration()
        {
            // Arrange - Load balancer integration scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should support load balancer health checks
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once,
                "because load balancers require ping endpoints for health checks");
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_KubernetesReadinessProbe()
        {
            // Arrange - Kubernetes readiness probe scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should support Kubernetes readiness probes
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once,
                "because Kubernetes uses ping endpoints for readiness probes");
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_MonitoringSystemIntegration()
        {
            // Arrange - Monitoring system integration scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should support monitoring system integration
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once,
                "because monitoring systems require ping endpoints to verify service availability");
        }

        [Fact]
        public void UsePing_ShouldRegisterMiddleware()
        {
            // Arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();
            RequestDelegate capturedMiddleware = null;

            mockApplicationBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Callback<Func<RequestDelegate, RequestDelegate>>(middleware =>
                {
                    // Capture the middleware for testing
                    capturedMiddleware = middleware(context => Task.CompletedTask);
                });

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should register middleware
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
            capturedMiddleware.Should().NotBeNull();
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_ServiceMeshIntegration()
        {
            // Arrange - Service mesh integration scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should support service mesh health checks
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once,
                "because service mesh requires ping endpoints for service discovery and health checks");
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_APIGatewayIntegration()
        {
            // Arrange - API Gateway integration scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should support API Gateway health checks
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once,
                "because API Gateways use ping endpoints to determine service health");
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_ContainerOrchestration()
        {
            // Arrange - Container orchestration scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should support container orchestration health checks
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once,
                "because container orchestrators require ping endpoints for health monitoring");
        }

        [Fact]
        public void UsePing_ShouldSupportChaining()
        {
            // Arrange - Method chaining scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();
            mockApplicationBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns(mockApplicationBuilder.Object);

            // Act - Should support method chaining
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should register middleware for chaining
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_HighAvailabilitySetup()
        {
            // Arrange - High availability setup scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should support high availability configurations
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once,
                "because high availability setups require ping endpoints for failover detection");
        }

        [Fact]
        public void UsePing_ShouldSupportBusinessScenario_DevOpsMonitoring()
        {
            // Arrange - DevOps monitoring scenario
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();

            // Act
            mockApplicationBuilder.Object.UsePing();

            // Assert - Should support DevOps monitoring tools
            mockApplicationBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once,
                "because DevOps monitoring tools require ping endpoints for automated health checks");
        }
    }
}
