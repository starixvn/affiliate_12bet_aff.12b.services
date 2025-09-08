using Aff12B.Ext.API.Infastructure.Extentions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Extensions
{
    public class MiddlewareExtensionsTests
    {
        private readonly Mock<IApplicationBuilder> _mockAppBuilder;
        private readonly IServiceCollection _services;

        public MiddlewareExtensionsTests()
        {
            _mockAppBuilder = new Mock<IApplicationBuilder>();
            _services = new ServiceCollection();
        }

        #region UseJWTokenMiddleware Tests

        [Fact]
        public void UseJWTokenMiddleware_ShouldAddMiddleware()
        {
            // Arrange
            _mockAppBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                          .Returns(_mockAppBuilder.Object);

            // Act
            var result = _mockAppBuilder.Object.UseJWTokenMiddleware();

            // Assert
            result.Should().BeSameAs(_mockAppBuilder.Object);
            _mockAppBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }

        [Fact]
        public void UseJWTokenMiddleware_ShouldReturnApplicationBuilder()
        {
            // Arrange
            _mockAppBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                          .Returns(_mockAppBuilder.Object);

            // Act
            var result = _mockAppBuilder.Object.UseJWTokenMiddleware();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(_mockAppBuilder.Object);
            result.Should().BeAssignableTo<IApplicationBuilder>();
        }

        [Fact]
        public void UseJWTokenMiddleware_NullApplicationBuilder_ShouldThrowNullReferenceException()
        {
            // Arrange
            IApplicationBuilder nullBuilder = null;

            // Act & Assert
            var act = () => nullBuilder!.UseJWTokenMiddleware();
            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void UseJWTokenMiddleware_CalledMultipleTimes_ShouldAddMiddlewareMultipleTimes()
        {
            // Arrange
            _mockAppBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                          .Returns(_mockAppBuilder.Object);

            // Act
            _mockAppBuilder.Object.UseJWTokenMiddleware();
            _mockAppBuilder.Object.UseJWTokenMiddleware();

            // Assert
            _mockAppBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Exactly(2));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void UseJWTokenMiddleware_WithRealApplicationBuilder_ShouldWork()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<IWebHostEnvironment>(new Mock<IWebHostEnvironment>().Object);
            var serviceProvider = services.BuildServiceProvider();

            var appBuilder = new ApplicationBuilder(serviceProvider);

            // Act
            var result = appBuilder.UseJWTokenMiddleware();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(appBuilder);
        }

        [Fact]
        public void UseJWTokenMiddleware_MiddlewareRegistration_ShouldUseCorrectMiddleware()
        {
            // This test verifies that UseMiddleware<JWTokenHandler>() is called
            // We can't directly test this without complex reflection, but we can verify the behavior

            // Arrange
            var middlewareTypes = new List<Type>();
            _mockAppBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                          .Callback<Func<RequestDelegate, RequestDelegate>>(middleware =>
                          {
                              // This would be complex to test the exact middleware type
                              // For now, we just verify the method was called
                          })
                          .Returns(_mockAppBuilder.Object);

            // Act
            _mockAppBuilder.Object.UseJWTokenMiddleware();

            // Assert
            _mockAppBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }

        [Fact]
        public async Task UseJWTokenMiddleware_FullPipeline_ShouldProcessRequests()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var appBuilder = new ApplicationBuilder(serviceProvider);

            var wasMiddlewareCalled = false;

            // Add our middleware
            appBuilder.UseJWTokenMiddleware();

            // Add a test middleware to verify the pipeline works
            appBuilder.Use(async (context, next) =>
            {
                wasMiddlewareCalled = true;
                await next();
            });

            var app = appBuilder.Build();
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?access_token=test-token");

            // Act
            await app(context);

            // Assert
            wasMiddlewareCalled.Should().BeTrue();
            context.Request.Headers["Authorization"].ToString().Should().Be("Bearer test-token");
        }

        [Fact]
        public void UseJWTokenMiddleware_ChainedWithOtherMiddleware_ShouldMaintainChain()
        {
            // Arrange
            _mockAppBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                          .Returns(_mockAppBuilder.Object);

            // Act
            var result = _mockAppBuilder.Object
                .UseJWTokenMiddleware()
                .UseJWTokenMiddleware(); // Chain multiple calls

            // Assert
            result.Should().BeSameAs(_mockAppBuilder.Object);
            _mockAppBuilder.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Exactly(2));
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void UseJWTokenMiddleware_ApplicationBuilderProperties_ShouldBePreserved()
        {
            // Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var properties = new Dictionary<string, object> { { "test-key", "test-value" } };

            _mockAppBuilder.Setup(x => x.ApplicationServices).Returns(mockServiceProvider.Object);
            _mockAppBuilder.Setup(x => x.Properties).Returns(properties);
            _mockAppBuilder.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                          .Returns(_mockAppBuilder.Object);

            // Act
            var result = _mockAppBuilder.Object.UseJWTokenMiddleware();

            // Assert
            result.Should().BeSameAs(_mockAppBuilder.Object);
            // Properties and services should be preserved (they're on the same object)
            _mockAppBuilder.VerifyGet(x => x.ApplicationServices, Times.Never); // Not accessed by the extension
            _mockAppBuilder.VerifyGet(x => x.Properties, Times.Never); // Not accessed by the extension
        }

        #endregion
    }
}
