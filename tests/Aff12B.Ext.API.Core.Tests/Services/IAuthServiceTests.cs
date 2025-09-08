using Aff12B.Ext.API.Core.Models;
using Aff12B.Ext.API.Core.Params;
using Aff12B.Ext.API.Core.Services;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Core.Tests.Services
{
    /// <summary>
    /// Unit tests for IAuthService interface contract
    /// Tests interface contract validation, method signatures, and implementation requirements
    /// </summary>
    public class IAuthServiceTests
    {
        [Fact]
        public void IAuthService_ShouldHaveCorrectInterface()
        {
            // Arrange
            var interfaceType = typeof(IAuthService);

            // Act & Assert
            interfaceType.IsInterface.Should().BeTrue();
            interfaceType.IsPublic.Should().BeTrue();
            interfaceType.IsAbstract.Should().BeTrue(); // Interfaces are abstract by definition
        }

        [Fact]
        public void IAuthService_ShouldHaveIsAuthenticatedMethod()
        {
            // Arrange
            var interfaceType = typeof(IAuthService);

            // Act
            var method = interfaceType.GetMethod("IsAuthenticated");

            // Assert
            method.Should().NotBeNull();
            method!.Name.Should().Be("IsAuthenticated");
            method.IsPublic.Should().BeTrue();
            method.IsAbstract.Should().BeTrue();
        }

        [Fact]
        public void IAuthService_IsAuthenticatedMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var interfaceType = typeof(IAuthService);

            // Act
            var method = interfaceType.GetMethod("IsAuthenticated");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be<Task<Tuple<bool, Token>>>();

            var parameters = method.GetParameters();
            parameters.Should().HaveCount(1);
            parameters[0].ParameterType.Should().Be<TokenRequest>();
            parameters[0].Name.Should().Be("request");
        }

        [Fact]
        public void IAuthService_ShouldHaveOnlyOneMethod()
        {
            // Arrange
            var interfaceType = typeof(IAuthService);

            // Act
            var methods = interfaceType.GetMethods();

            // Assert
            methods.Should().HaveCount(1);
            methods[0].Name.Should().Be("IsAuthenticated");
        }

        [Fact]
        public void IAuthService_ShouldHaveCorrectNamespace()
        {
            // Arrange
            var interfaceType = typeof(IAuthService);

            // Act & Assert
            interfaceType.Namespace.Should().Be("Aff12B.Ext.API.Core.Services");
        }

        [Fact]
        public void IAuthService_ShouldBeImplementableByTestDouble()
        {
            // Arrange & Act
            var testImplementation = new TestAuthServiceImplementation();

            // Assert
            testImplementation.Should().BeAssignableTo<IAuthService>();
        }

        [Fact]
        public async Task IAuthService_ShouldSupportPolymorphism()
        {
            // Arrange
            IAuthService authService1 = new TestAuthServiceImplementation();
            IAuthService authService2 = new AlternativeAuthServiceImplementation();

            var request = new TokenRequest { Username = "test", Password = "test" };

            // Act
            var result1 = await authService1.IsAuthenticated(request);
            var result2 = await authService2.IsAuthenticated(request);

            // Assert
            authService1.Should().BeAssignableTo<IAuthService>();
            authService2.Should().BeAssignableTo<IAuthService>();

            result1.Should().NotBeNull();
            result2.Should().NotBeNull();

            result1.Item1.Should().BeTrue(); // TestAuthServiceImplementation returns true
            result2.Item1.Should().BeFalse(); // AlternativeAuthServiceImplementation returns false
        }

        [Fact]
        public async Task IAuthService_ShouldSupportCollectionOperations()
        {
            // Arrange
            var authServices = new List<IAuthService>
            {
                new TestAuthServiceImplementation(),
                new AlternativeAuthServiceImplementation()
            };

            var request = new TokenRequest { Username = "test", Password = "test" };

            // Act
            var results = new List<Tuple<bool, Token>>();
            foreach (var service in authServices)
            {
                var result = await service.IsAuthenticated(request);
                results.Add(result);
            }

            // Assert
            results.Should().HaveCount(2);
            results[0].Item1.Should().BeTrue();
            results[1].Item1.Should().BeFalse();
        }

        [Fact]
        public async Task IAuthService_ShouldSupportAsyncOperations()
        {
            // Arrange
            IAuthService authService = new TestAuthServiceImplementation();
            var request = new TokenRequest { Username = "async_test", Password = "async_pass" };

            // Act
            var task = authService.IsAuthenticated(request);
            var result = await task;

            // Assert
            task.Should().BeAssignableTo<Task<Tuple<bool, Token>>>();
            result.Should().NotBeNull();
            result.Item1.Should().BeTrue();
            result.Item2.Should().NotBeNull();
        }

        [Fact]
        public void IAuthService_IsAuthenticatedMethod_ShouldReturnTaskOfTuple()
        {
            // Arrange
            var interfaceType = typeof(IAuthService);

            // Act
            var method = interfaceType.GetMethod("IsAuthenticated");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task<Tuple<bool, Token>>));
            method.ReturnType.IsGenericType.Should().BeTrue();
            method.ReturnType.GetGenericTypeDefinition().Should().Be(typeof(Task<>));

            var taskGenericArg = method.ReturnType.GetGenericArguments()[0];
            taskGenericArg.Should().Be(typeof(Tuple<bool, Token>));
        }

        [Fact]
        public async Task IAuthService_ShouldHandleValidAuthenticationScenario()
        {
            // Arrange
            IAuthService authService = new TestAuthServiceImplementation();
            var validRequest = new TokenRequest
            {
                Username = "validuser",
                Password = "validpass",
                IsShowIpvn = true
            };

            // Act
            var result = await authService.IsAuthenticated(validRequest);

            // Assert
            result.Should().NotBeNull();
            result.Item1.Should().BeTrue();
            result.Item2.Should().NotBeNull();
            result.Item2.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task IAuthService_ShouldHandleInvalidAuthenticationScenario()
        {
            // Arrange
            IAuthService authService = new AlternativeAuthServiceImplementation();
            var invalidRequest = new TokenRequest
            {
                Username = "invaliduser",
                Password = "invalidpass",
                IsShowIpvn = true
            };

            // Act
            var result = await authService.IsAuthenticated(invalidRequest);

            // Assert
            result.Should().NotBeNull();
            result.Item1.Should().BeFalse();
            result.Item2.Should().NotBeNull();
        }

        [Fact]
        public void IAuthService_ShouldUseCorrectParameterTypes()
        {
            // Arrange
            var interfaceType = typeof(IAuthService);

            // Act
            var method = interfaceType.GetMethod("IsAuthenticated");
            var parameter = method!.GetParameters()[0];

            // Assert
            parameter.ParameterType.Should().Be<TokenRequest>();
            parameter.Name.Should().Be("request");
        }

        [Fact]
        public void IAuthService_ShouldUseCorrectReturnTypes()
        {
            // Arrange
            var interfaceType = typeof(IAuthService);

            // Act
            var method = interfaceType.GetMethod("IsAuthenticated");

            // Assert
            method!.ReturnType.Should().Be<Task<Tuple<bool, Token>>>();

            // Verify the tuple contains correct types
            var tupleType = method.ReturnType.GetGenericArguments()[0];
            var tupleGenericArgs = tupleType.GetGenericArguments();
            tupleGenericArgs[0].Should().Be<bool>();
            tupleGenericArgs[1].Should().Be<Token>();
        }

        [Fact]
        public async Task IAuthService_ShouldSupportLinqOperations()
        {
            // Arrange
            var authServices = new List<IAuthService>
            {
                new TestAuthServiceImplementation(),
                new AlternativeAuthServiceImplementation(),
                new TestAuthServiceImplementation()
            };

            var request = new TokenRequest { Username = "linq_test", Password = "linq_pass" };

            // Act
            var results = new List<bool>();
            foreach (var service in authServices)
            {
                var result = await service.IsAuthenticated(request);
                results.Add(result.Item1);
            }

            var successfulAuths = results.Where(r => r).Count();
            var failedAuths = results.Where(r => !r).Count();

            // Assert
            successfulAuths.Should().Be(2); // Two TestAuthServiceImplementation instances
            failedAuths.Should().Be(1); // One AlternativeAuthServiceImplementation instance
        }

        // Test implementation classes for validation
        private class TestAuthServiceImplementation : IAuthService
        {
            public async Task<Tuple<bool, Token>> IsAuthenticated(TokenRequest request)
            {
                await Task.Delay(1); // Simulate async operation

                var token = new Token
                {
                    AccessToken = "test-access-token",
                    TokenType = "Bearer",
                    ExpiresIn = 3600
                };

                return new Tuple<bool, Token>(true, token);
            }
        }

        private class AlternativeAuthServiceImplementation : IAuthService
        {
            public async Task<Tuple<bool, Token>> IsAuthenticated(TokenRequest request)
            {
                await Task.Delay(1); // Simulate async operation

                var emptyToken = new Token
                {
                    AccessToken = "",
                    TokenType = "",
                    ExpiresIn = 0
                };

                return new Tuple<bool, Token>(false, emptyToken);
            }
        }
    }
}
