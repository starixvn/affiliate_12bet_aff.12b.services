using Aff12B.Common.Infrastructure.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Net;
using Xunit;

namespace Aff12B.Common.Infrastructure.Tests.Middleware
{
    /// <summary>
    /// Unit tests for WhoAmIHealthCheck class.
    /// Tests server information health check functionality.
    /// </summary>
    public class WhoAmIHealthCheckTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IHostEnvironment> _mockHostEnvironment;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<ConnectionInfo> _mockConnection;
        private readonly Mock<HttpRequest> _mockRequest;
        private readonly WhoAmIHealthCheck _healthCheck;

        public WhoAmIHealthCheckTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHostEnvironment = new Mock<IHostEnvironment>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockConnection = new Mock<ConnectionInfo>();
            _mockRequest = new Mock<HttpRequest>();

            // Setup basic mocks
            _mockHostEnvironment.Setup(x => x.EnvironmentName).Returns("Test");
            _mockHttpContext.Setup(x => x.Connection).Returns(_mockConnection.Object);
            _mockHttpContext.Setup(x => x.Request).Returns(_mockRequest.Object);
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

            // Setup connection info
            _mockConnection.Setup(x => x.LocalIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
            _mockConnection.Setup(x => x.LocalPort).Returns(8080);
            _mockConnection.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("192.168.1.100"));

            // Setup request info
            _mockRequest.Setup(x => x.Host).Returns(new HostString("localhost:8080"));
            _mockRequest.Setup(x => x.Scheme).Returns("https");

            _healthCheck = new WhoAmIHealthCheck(_mockHttpContextAccessor.Object, _mockHostEnvironment.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var healthCheck = new WhoAmIHealthCheck(_mockHttpContextAccessor.Object, _mockHostEnvironment.Object);
            healthCheck.Should().NotBeNull();
        }

        [Fact]
        public void WhoAmIHealthCheck_ShouldImplementIHealthCheck()
        {
            // Act & Assert - Should implement required interface
            _healthCheck.Should().BeAssignableTo<IHealthCheck>();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldReturnHealthyResult()
        {
            // Arrange
            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should return healthy status
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Description.Should().Be("Server Info");
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeEnvironmentInfo()
        {
            // Arrange
            _mockHostEnvironment.Setup(x => x.EnvironmentName).Returns("Production");
            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should include environment information
            result.Data.Should().ContainKey("Env");
            result.Data["Env"].Should().Be("Production");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeServerIPInfo()
        {
            // Arrange
            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should include server IP information
            result.Data.Should().ContainKey("Host Name");
            result.Data["Host Name"].Should().NotBeNull();
            result.Data["Host Name"].Should().BeOfType<string>();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeConnectionInfo()
        {
            // Arrange
            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should include connection information
            result.Data.Should().ContainKey("Local Ip");
            result.Data.Should().ContainKey("Local Port");
            result.Data.Should().ContainKey("Client Ip");

            result.Data["Local Ip"].Should().Be("127.0.0.1");
            result.Data["Local Port"].Should().Be(8080);
            result.Data["Client Ip"].Should().Be("192.168.1.100");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeRequestInfo()
        {
            // Arrange
            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should include request information
            result.Data.Should().ContainKey("Host");
            result.Data.Should().ContainKey("Schema");

            result.Data["Host"].Should().Be(new HostString("localhost:8080"));
            result.Data["Schema"].Should().Be("https");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeTimingInfo()
        {
            // Arrange
            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should include timing information
            result.Data.Should().ContainKey("Server Time");
            result.Data.Should().ContainKey("Last Updated");

            result.Data["Server Time"].Should().NotBeNull();
            result.Data["Last Updated"].Should().NotBeNull();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportDevelopmentEnvironment()
        {
            // Arrange - Development environment scenario
            _mockHostEnvironment.Setup(x => x.EnvironmentName).Returns("Development");
            _mockConnection.Setup(x => x.LocalIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
            _mockConnection.Setup(x => x.LocalPort).Returns(5000);
            _mockRequest.Setup(x => x.Host).Returns(new HostString("localhost:5000"));
            _mockRequest.Setup(x => x.Scheme).Returns("http");

            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should support development environment
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data["Env"].Should().Be("Development");
            result.Data["Local Port"].Should().Be(5000);
            result.Data["Schema"].Should().Be("http");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportProductionEnvironment()
        {
            // Arrange - Production environment scenario
            _mockHostEnvironment.Setup(x => x.EnvironmentName).Returns("Production");
            _mockConnection.Setup(x => x.LocalIpAddress).Returns(IPAddress.Parse("10.0.0.5"));
            _mockConnection.Setup(x => x.LocalPort).Returns(443);
            _mockConnection.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("203.0.113.1"));
            _mockRequest.Setup(x => x.Host).Returns(new HostString("api.company.com"));
            _mockRequest.Setup(x => x.Scheme).Returns("https");

            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should support production environment
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data["Env"].Should().Be("Production");
            result.Data["Local Ip"].Should().Be("10.0.0.5");
            result.Data["Local Port"].Should().Be(443);
            result.Data["Client Ip"].Should().Be("203.0.113.1");
            result.Data["Host"].Should().Be(new HostString("api.company.com"));
            result.Data["Schema"].Should().Be("https");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportStagingEnvironment()
        {
            // Arrange - Staging environment scenario
            _mockHostEnvironment.Setup(x => x.EnvironmentName).Returns("Staging");
            _mockConnection.Setup(x => x.LocalIpAddress).Returns(IPAddress.Parse("172.16.0.10"));
            _mockConnection.Setup(x => x.LocalPort).Returns(8443);
            _mockConnection.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("172.16.0.100"));
            _mockRequest.Setup(x => x.Host).Returns(new HostString("staging-api.company.com:8443"));
            _mockRequest.Setup(x => x.Scheme).Returns("https");

            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should support staging environment
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data["Env"].Should().Be("Staging");
            result.Data["Local Ip"].Should().Be("172.16.0.10");
            result.Data["Local Port"].Should().Be(8443);
            result.Data["Client Ip"].Should().Be("172.16.0.100");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportIPv6Addresses()
        {
            // Arrange - IPv6 scenario
            _mockConnection.Setup(x => x.LocalIpAddress).Returns(IPAddress.Parse("::1"));
            _mockConnection.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("2001:db8::1"));

            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should support IPv6 addresses
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data["Local Ip"].Should().Be("::1");
            result.Data["Client Ip"].Should().Be("2001:db8::1");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportLoadBalancerScenario()
        {
            // Arrange - Load balancer scenario with X-Forwarded headers
            _mockConnection.Setup(x => x.LocalIpAddress).Returns(IPAddress.Parse("10.0.1.5"));
            _mockConnection.Setup(x => x.LocalPort).Returns(80);
            _mockConnection.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("10.0.1.1")); // Load balancer IP
            _mockRequest.Setup(x => x.Host).Returns(new HostString("api.company.com"));
            _mockRequest.Setup(x => x.Scheme).Returns("https"); // HTTPS terminated at load balancer

            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should support load balancer scenario
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data["Local Ip"].Should().Be("10.0.1.5");
            result.Data["Local Port"].Should().Be(80);
            result.Data["Client Ip"].Should().Be("10.0.1.1");
            result.Data["Schema"].Should().Be("https");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeAllRequiredDataKeys()
        {
            // Arrange
            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Should include all required data keys
            var expectedKeys = new[]
            {
                "Env",
                "Host Name",
                "Local Ip",
                "Local Port",
                "Client Ip",
                "Server Time",
                "Last Updated",
                "Host",
                "Schema"
            };

            foreach (var key in expectedKeys)
            {
                result.Data.Should().ContainKey(key, $"Health check should include {key}");
            }
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldHandleCancellationToken()
        {
            // Arrange
            var context = new HealthCheckContext();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context, cancellationToken);

            // Assert - Should complete successfully with cancellation token
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldProvideReadOnlyData()
        {
            // Arrange
            var context = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(context);

            // Assert - Data should be read-only
            result.Data.Should().NotBeNull();
            result.Data.GetType().Name.Should().Contain("ReadOnly");
        }
    }
}
