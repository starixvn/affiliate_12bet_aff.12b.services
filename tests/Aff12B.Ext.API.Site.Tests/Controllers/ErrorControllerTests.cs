using Aff12B.Ext.API.Site.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests.Controllers
{
    /// <summary>
    /// Comprehensive unit tests for ErrorController class.
    /// Business-focused scenarios for error handling endpoints in API responses.
    /// Covers error responses, authorization failures, and whitelist violations.
    /// </summary>
    public class ErrorControllerTests
    {
        private readonly ErrorController _controller;

        public ErrorControllerTests()
        {
            _controller = new ErrorController();
        }

        [Fact]
        public void Index_ShouldReturnJsonResult_WithErrorMessage()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Should().NotBeNull();

            var value = jsonResult!.Value;
            value.Should().NotBeNull();
            value.ToString().Should().Contain("Error");
        }

        [Fact]
        public void Index_ShouldReturnGenericErrorResponse()
        {
            // Arrange - Generic error scenario (500 Internal Server Error)

            // Act
            var result = _controller.Index();

            // Assert - Should return generic error message
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;

            // Verify the response structure matches expected error format
            var responseData = jsonResult!.Value;
            responseData.Should().NotBeNull();

            // Use reflection to check the anonymous object properties
            var messageProperty = responseData!.GetType().GetProperty("Message");
            messageProperty.Should().NotBeNull();
            messageProperty!.GetValue(responseData).Should().Be("Error");
        }

        [Fact]
        public void UnAuthorized_ShouldReturnJsonResult_WithUnAuthorizedMessage()
        {
            // Act
            var result = _controller.UnAuthorized();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Should().NotBeNull();

            var value = jsonResult!.Value;
            value.Should().NotBeNull();
            value.ToString().Should().Contain("UnAuthorized");
        }

        [Fact]
        public void UnAuthorized_ShouldReturnAuthorizationErrorResponse()
        {
            // Arrange - Authentication/Authorization failure scenario (401 Unauthorized)

            // Act
            var result = _controller.UnAuthorized();

            // Assert - Should return unauthorized message
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;

            // Verify the response structure
            var responseData = jsonResult!.Value;
            responseData.Should().NotBeNull();

            var messageProperty = responseData!.GetType().GetProperty("Message");
            messageProperty.Should().NotBeNull();
            messageProperty!.GetValue(responseData).Should().Be("UnAuthorized");
        }

        [Fact]
        public void NotWhiteList_ShouldReturnJsonResult_WithNotWhiteListMessage()
        {
            // Act
            var result = _controller.NotWhiteList();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Should().NotBeNull();

            var value = jsonResult!.Value;
            value.Should().NotBeNull();
            value.ToString().Should().Contain("NotWhiteList");
        }

        [Fact]
        public void NotWhiteList_ShouldReturnWhitelistViolationResponse()
        {
            // Arrange - IP whitelist violation scenario (403 Forbidden)

            // Act
            var result = _controller.NotWhiteList();

            // Assert - Should return whitelist violation message
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;

            // Verify the response structure
            var responseData = jsonResult!.Value;
            responseData.Should().NotBeNull();

            var messageProperty = responseData!.GetType().GetProperty("Message");
            messageProperty.Should().NotBeNull();
            messageProperty!.GetValue(responseData).Should().Be("NotWhiteList");
        }

        [Fact]
        public void AllErrorMethods_ShouldReturnConsistentJsonStructure()
        {
            // Arrange - Test all error endpoints for consistent response structure

            // Act
            var indexResult = _controller.Index();
            var unauthorizedResult = _controller.UnAuthorized();
            var notWhiteListResult = _controller.NotWhiteList();

            // Assert - All should return JsonResult with Message property
            var results = new[] { indexResult, unauthorizedResult, notWhiteListResult };

            foreach (var result in results)
            {
                result.Should().BeOfType<JsonResult>();
                var jsonResult = result as JsonResult;
                jsonResult!.Value.Should().NotBeNull();

                var messageProperty = jsonResult.Value!.GetType().GetProperty("Message");
                messageProperty.Should().NotBeNull();
                messageProperty!.GetValue(jsonResult.Value).Should().NotBeNull();
            }
        }

        [Fact]
        public void ErrorController_ShouldHaveCorrectAttributes()
        {
            // Arrange - Check controller attributes for security and routing
            var controllerType = typeof(ErrorController);

            // Act & Assert - Verify controller has correct attributes
            var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;
            var allowAnonymousAttribute = controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

            // Verify routing configuration
            routeAttribute.Should().NotBeNull();
            routeAttribute!.Template.Should().Be("api/[controller]/[action]");

            // Verify anonymous access is allowed (important for error endpoints)
            allowAnonymousAttribute.Should().NotBeNull();
        }

        [Fact]
        public void ErrorMethods_ShouldHaveCorrectHttpGetAttributes()
        {
            // Arrange - Check method attributes
            var controllerType = typeof(ErrorController);
            var methods = new[] { "Index", "UnAuthorized", "NotWhiteList" };

            // Act & Assert - Verify all methods have HttpGet attributes
            foreach (var methodName in methods)
            {
                var method = controllerType.GetMethod(methodName);
                method.Should().NotBeNull();

                var httpGetAttribute = method!.GetCustomAttributes(typeof(HttpGetAttribute), false).FirstOrDefault();
                httpGetAttribute.Should().NotBeNull($"Method {methodName} should have HttpGet attribute");
            }
        }

        [Fact]
        public void ErrorResponses_ShouldBeDifferentiable()
        {
            // Arrange - Business scenario: Different error types should be distinguishable

            // Act
            var genericError = _controller.Index();
            var authError = _controller.UnAuthorized();
            var whitelistError = _controller.NotWhiteList();

            // Assert - Each error should have a unique message
            var genericMessage = GetMessageFromJsonResult(genericError);
            var authMessage = GetMessageFromJsonResult(authError);
            var whitelistMessage = GetMessageFromJsonResult(whitelistError);

            genericMessage.Should().Be("Error");
            authMessage.Should().Be("UnAuthorized");
            whitelistMessage.Should().Be("NotWhiteList");

            // All messages should be different
            var messages = new[] { genericMessage, authMessage, whitelistMessage };
            messages.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void ErrorController_ShouldBeStateless()
        {
            // Arrange - Test controller statelessness (important for scalability)
            var controller1 = new ErrorController();
            var controller2 = new ErrorController();

            // Act - Call same method on different instances
            var result1 = controller1.Index();
            var result2 = controller2.Index();

            // Assert - Results should be identical (stateless behavior)
            var message1 = GetMessageFromJsonResult(result1);
            var message2 = GetMessageFromJsonResult(result2);

            message1.Should().Be(message2);
        }

        [Fact]
        public async Task ErrorController_ShouldHandleMultipleConcurrentCalls()
        {
            // Arrange - Test thread safety (important for high-traffic scenarios)
            var tasks = new List<Task<string>>();

            // Act - Make multiple concurrent calls
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var result = _controller.Index();
                    return GetMessageFromJsonResult(result);
                }));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - All results should be consistent
            results.Should().AllBe("Error");
            results.Should().HaveCount(10);
        }

        // Helper method to extract message from JsonResult
        private static string GetMessageFromJsonResult(IActionResult result)
        {
            var jsonResult = result as JsonResult;
            jsonResult.Should().NotBeNull();

            var responseData = jsonResult!.Value;
            responseData.Should().NotBeNull();

            var messageProperty = responseData!.GetType().GetProperty("Message");
            messageProperty.Should().NotBeNull();

            return messageProperty!.GetValue(responseData)?.ToString() ?? string.Empty;
        }
    }
}
