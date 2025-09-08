using Aff12B.Common.DataDomain.Api;
using Aff12B.Common.DataDomain.Params;
using Aff12B.Ext.API.Site.Tests.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests.Integration
{
    /// <summary>
    /// Base class for integration tests providing common setup and utilities
    /// </summary>
    [Trait("Category", "Integration")]
    [Collection("IntegrationTestCollection")]
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly TestWebApplicationFactory Factory;
        protected readonly HttpClient Client;
        private bool _disposed = false;

        protected IntegrationTestBase(TestWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }

        /// <summary>
        /// Gets an authenticated HTTP client with JWT token
        /// </summary>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <returns>Authenticated HTTP client</returns>
        protected async Task<HttpClient> GetAuthenticatedClientAsync(string username = "testuser", string password = "password")
        {
            // Get JWT token
            var token = await GetJwtTokenAsync(username, password);

            // Create authenticated client
            return ApiTestUtilities.CreateAuthenticatedClient(Factory, token);
        }

        /// <summary>
        /// Gets JWT token for authentication
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>JWT token string</returns>
        protected async Task<string> GetJwtTokenAsync(string username = "testuser", string password = "password")
        {
            var tokenRequest = ApiTestUtilities.CreateTokenRequest(username, password);
            var content = ApiTestUtilities.CreateJsonContent(tokenRequest);

            var response = await Client.PostAsync("/api/auth/token", content);
            response.EnsureSuccessStatusCode();

            var tokenInfo = await ApiTestUtilities.DeserializeResponse<TokenInfo>(response);
            return tokenInfo.AccessToken;
        }

        /// <summary>
        /// Creates a service scope for dependency injection testing
        /// </summary>
        /// <returns>Service scope</returns>
        protected IServiceScope CreateScope()
        {
            return ApiTestUtilities.CreateTestScope(Factory);
        }

        /// <summary>
        /// Gets a service from the test container
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service instance</returns>
        protected T GetService<T>() where T : notnull
        {
            using var scope = CreateScope();
            return ApiTestUtilities.GetService<T>(scope);
        }

        /// <summary>
        /// Creates test affiliate parameters with default values
        /// </summary>
        /// <param name="custId">Customer ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="affiliateId">Affiliate ID</param>
        /// <returns>Affiliate parameters</returns>
        protected AffiliateParam CreateTestAffiliateParam(int custId = 12345, int siteId = 1, int affiliateId = 1)
        {
            return ApiTestUtilities.CreateAffiliateParam(custId, siteId, affiliateId);
        }

        /// <summary>
        /// Creates test batch count parameters with default values
        /// </summary>
        /// <param name="batchCount">Batch count</param>
        /// <param name="daysBack">Number of days back for from date</param>
        /// <returns>Batch count parameters</returns>
        protected BatchCountParam CreateTestBatchCountParam(int batchCount = 100, int daysBack = 1)
        {
            return ApiTestUtilities.CreateBatchCountParam(
                batchCount,
                DateTime.UtcNow.AddDays(-daysBack),
                DateTime.UtcNow
            );
        }

        /// <summary>
        /// Validates that an API response has the expected structure and status
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <param name="expectedStatusCode">Expected status code</param>
        /// <returns>True if response is valid</returns>
        protected async Task<bool> ValidateResponseAsync(
            HttpResponseMessage response,
            System.Net.HttpStatusCode expectedStatusCode = System.Net.HttpStatusCode.OK)
        {
            return await ApiTestUtilities.ValidateApiResponse(response, expectedStatusCode);
        }

        /// <summary>
        /// Deserializes response content to the specified type
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <param name="response">HTTP response</param>
        /// <returns>Deserialized object</returns>
        protected async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            return await ApiTestUtilities.DeserializeResponse<T>(response);
        }

        /// <summary>
        /// Creates JSON content for HTTP requests
        /// </summary>
        /// <param name="data">Object to serialize</param>
        /// <returns>JSON string content</returns>
        protected StringContent CreateJsonContent(object data)
        {
            return ApiTestUtilities.CreateJsonContent(data);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Client?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
