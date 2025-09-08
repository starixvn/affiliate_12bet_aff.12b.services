using Aff12B.Common.Infrastructure.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit;

namespace Aff12B.Common.Infrastructure.Tests.Helpers
{
    /// <summary>
    /// Comprehensive unit tests for ErrorMessageHelper class.
    /// Business-focused scenarios for error message generation in production environments.
    /// Simplified approach focusing on core functionality and business scenarios.
    /// </summary>
    public class ErrorMessageHelperSimplifiedTests
    {
        [Fact]
        public void GenerateMessage_ShouldIncludeBasicExceptionInfo()
        {
            // Arrange
            var exception = new InvalidOperationException("Customer data validation failed");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception);

            // Assert
            result.Should().Contain("ErrMsg: Customer data validation failed");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("Message: Customer data validation failed");
            result.Should().Contain("DATABASE INFO");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleHttpContextScenario()
        {
            // Arrange - Web request error scenario
            var context = CreateMockHttpContext("casino-api.com", "/api/customers", "192.168.1.100");
            var exception = new UnauthorizedAccessException("JWT token expired");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception, context);

            // Assert - HTTP context details
            result.Should().Contain("ErrMsg: JWT token expired");
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("Host: casino-api.com");
            result.Should().Contain("Path: /api/customers");
            result.Should().Contain("Remote IP: 192.168.1.100");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("DATABASE INFO");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleNestedExceptionsScenario()
        {
            // Arrange - Nested exception scenario (Business → Data → Connection)
            var innermost = new InvalidOperationException("Database connection timeout");
            var middle = new ApplicationException("Data retrieval failed", innermost);
            var outermost = new BusinessException("Customer bonus calculation failed", middle);

            // Act
            var result = ErrorMessageHelper.GenerateMessage(outermost);

            // Assert - Nested exception handling
            result.Should().Contain("ErrMsg: Customer bonus calculation failed");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("Message: Customer bonus calculation failed");
            result.Should().Contain("Message: Data retrieval failed"); // Inner exception
            result.Should().Contain("Message: Database connection timeout"); // Innermost exception
        }

        [Fact]
        public void GenerateMessage_ShouldLimitInnerExceptionDepth()
        {
            // Arrange - Deep nested exceptions (more than 3 levels)
            var level4 = new InvalidOperationException("Level 4: Connection pool exhausted");
            var level3 = new ApplicationException("Level 3: Database query timeout", level4);
            var level2 = new InvalidDataException("Level 2: Data validation failed", level3);
            var level1 = new ArgumentException("Level 1: Invalid customer ID", level2);
            var level0 = new BusinessException("Level 0: Customer processing failed", level1);

            // Act
            var result = ErrorMessageHelper.GenerateMessage(level0);

            // Assert - Should limit to 3 inner exception levels
            result.Should().Contain("Message: Level 0: Customer processing failed");
            result.Should().Contain("Message: Level 1: Invalid customer ID");
            result.Should().Contain("Message: Level 2: Data validation failed");
            result.Should().Contain("Message: Level 3: Database query timeout");
            result.Should().NotContain("Message: Level 4: Connection pool exhausted"); // Should be limited
        }

        [Fact]
        public void GenerateMessage_ShouldHandleNullHttpContext()
        {
            // Arrange
            var exception = new Exception("Test exception without HTTP context");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception, null);

            // Assert - Should handle null context gracefully
            result.Should().Contain("ErrMsg: Test exception without HTTP context");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("DATABASE INFO");
            result.Should().NotContain("REQUEST INFO"); // Should not include request info when context is null
        }

        [Fact]
        public void GenerateMessage_ShouldHandleComplexBusinessScenario()
        {
            // Arrange - Complex production scenario: Bonus calculation failure
            var context = CreateMockHttpContext("vip-casino.com", "/api/bonus/calculate", "203.0.113.45");
            var exception = new BusinessException("VIP bonus calculation failed for high-roller customer");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception, context);

            // Assert - Complex scenario validation
            result.Should().Contain("ErrMsg: VIP bonus calculation failed for high-roller customer");
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("Host: vip-casino.com");
            result.Should().Contain("Path: /api/bonus/calculate");
            result.Should().Contain("Remote IP: 203.0.113.45");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("DATABASE INFO");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleArgumentExceptionScenario()
        {
            // Arrange - Invalid argument scenario
            var exception = new ArgumentException("Customer ID must be positive", "customerId");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception);

            // Assert
            result.Should().Contain("ErrMsg: Customer ID must be positive");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("Message: Customer ID must be positive");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleTimeoutExceptionScenario()
        {
            // Arrange - Timeout scenario
            var exception = new TimeoutException("API request timeout after 30 seconds");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception);

            // Assert
            result.Should().Contain("ErrMsg: API request timeout after 30 seconds");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("Message: API request timeout after 30 seconds");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleNullReferenceExceptionScenario()
        {
            // Arrange - Null reference scenario
            var exception = new NullReferenceException("Customer object is null during bonus processing");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception);

            // Assert
            result.Should().Contain("ErrMsg: Customer object is null during bonus processing");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("Message: Customer object is null during bonus processing");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleHttpRequestExceptionScenario()
        {
            // Arrange - HTTP request failure scenario
            var context = CreateMockHttpContext("api.external-provider.com", "/validate-player", "10.0.0.1");
            var exception = new HttpRequestException("External provider API is unavailable");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception, context);

            // Assert
            result.Should().Contain("ErrMsg: External provider API is unavailable");
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("Host: api.external-provider.com");
            result.Should().Contain("Path: /validate-player");
            result.Should().Contain("EXCEPTION INFO");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleFormatExceptionScenario()
        {
            // Arrange - Data format error scenario
            var exception = new FormatException("Invalid date format in customer registration data");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception);

            // Assert
            result.Should().Contain("ErrMsg: Invalid date format in customer registration data");
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("Message: Invalid date format in customer registration data");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleUnauthorizedAccessExceptionScenario()
        {
            // Arrange - Authorization failure scenario
            var context = CreateMockHttpContext("secure-api.casino.com", "/admin/reports", "172.16.0.10");
            var exception = new UnauthorizedAccessException("Insufficient permissions for admin reports");

            // Act
            var result = ErrorMessageHelper.GenerateMessage(exception, context);

            // Assert
            result.Should().Contain("ErrMsg: Insufficient permissions for admin reports");
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("Host: secure-api.casino.com");
            result.Should().Contain("Path: /admin/reports");
            result.Should().Contain("Remote IP: 172.16.0.10");
        }

        // Helper method to create mock HttpContext
        private static HttpContext CreateMockHttpContext(string host, string path, string remoteIp)
        {
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString(host);
            context.Request.Path = path;
            context.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp);
            return context;
        }

        // Custom business exception for testing
        private class BusinessException : Exception
        {
            public BusinessException(string message) : base(message) { }
            public BusinessException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
