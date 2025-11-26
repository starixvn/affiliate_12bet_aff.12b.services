using Aff12B.Common.DataDomain.Params;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Aff12B.Ext.API.Site.Tests.TestUtilities
{
    /// <summary>
    /// Utilities for API testing including authentication, request creation, and response parsing
    /// </summary>
    public static class ApiTestUtilities
    {
        /// <summary>
        /// Creates a test HTTP client with authentication token
        /// </summary>
        /// <param name="factory">Web application factory</param>
        /// <param name="token">JWT token for authentication</param>
        /// <returns>Configured HTTP client</returns>
        public static HttpClient CreateAuthenticatedClient(WebApplicationFactory<Aff12B.Ext.API.Site.Startup> factory, string token)
        {
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        /// <summary>
        /// Creates a test HTTP client with basic authentication
        /// </summary>
        /// <param name="factory">Web application factory</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Configured HTTP client</returns>
        public static HttpClient CreateBasicAuthClient(WebApplicationFactory<Aff12B.Ext.API.Site.Startup> factory, string username, string password)
        {
            var client = factory.CreateClient();
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            return client;
        }

        /// <summary>
        /// Creates JSON content for HTTP requests
        /// </summary>
        /// <param name="data">Object to serialize</param>
        /// <returns>JSON string content</returns>
        public static StringContent CreateJsonContent(object data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Deserializes HTTP response content to specified type
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <param name="response">HTTP response</param>
        /// <returns>Deserialized object</returns>
        public static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result ?? throw new InvalidOperationException("Failed to deserialize response content");
        }

        /// <summary>
        /// Creates a test token request
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Token request object</returns>
        public static TokenRequest CreateTokenRequest(string username = "testuser", string password = "password")
        {
            return new TokenRequest
            {
                Username = username,
                Password = password
            };
        }

        /// <summary>
        /// Creates test affiliate parameters
        /// </summary>
        /// <param name="custId">Customer ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="affiliateId">Affiliate ID</param>
        /// <returns>Affiliate parameters</returns>
        public static AffiliateParam CreateAffiliateParam(
            int custId = 12345,
            int siteId = 1,
            int affiliateId = 1)
        {
            return new AffiliateParam
            {
                Player_CustId = custId,
                SiteId = siteId,
                AffId = affiliateId,
                UserName = "Test Affiliate",
                ParentAff = "Parent Affiliate",
                AffCode = "AFF001",
                Player_Username = "testaffiliate",
                FirstName = "Test",
                LastName = "Affiliate",
                Phone = "+1234567890",
                Email = "test@affiliate.com",
                Status = "Active"
            };
        }

        /// <summary>
        /// Creates test batch count parameters
        /// </summary>
        /// <param name="batchCount">Batch count</param>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <returns>Batch count parameters</returns>
        public static BatchCountParam CreateBatchCountParam(
            int batchCount = 100,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            return new BatchCountParam
            {
                BatchCount = batchCount,
                FromDate = fromDate ?? DateTime.UtcNow.AddDays(-1),
                ToDate = toDate ?? DateTime.UtcNow,
                SQ = 1,
                CreateTime = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates query string from parameters
        /// </summary>
        /// <param name="parameters">Dictionary of parameters</param>
        /// <returns>Query string</returns>
        public static string CreateQueryString(Dictionary<string, object> parameters)
        {
            var queryParams = new List<string>();
            foreach (var param in parameters)
            {
                if (param.Value != null)
                {
                    var value = param.Value is DateTime dateTime
                        ? dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                        : param.Value.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        queryParams.Add($"{param.Key}={Uri.EscapeDataString(value)}");
                    }
                }
            }
            return string.Join("&", queryParams);
        }

        /// <summary>
        /// Creates a URL with query parameters for affiliate customers
        /// </summary>
        /// <param name="baseUrl">Base URL</param>
        /// <param name="param">Affiliate parameters</param>
        /// <returns>Complete URL with query string</returns>
        public static string CreateAffCustomerUrl(string baseUrl, AffiliateParam param)
        {
            var parameters = new Dictionary<string, object>
            {
                ["custId"] = param.CustId,
                ["siteId"] = param.SiteId,
                ["affiliateId"] = param.AffId,
                ["affiliateName"] = param.UserName ?? "",
                ["affiliateCode"] = param.AffCode ?? ""
            };

            var queryString = CreateQueryString(parameters);
            return $"{baseUrl}?{queryString}";
        }

        /// <summary>
        /// Creates a URL with query parameters for batch operations
        /// </summary>
        /// <param name="baseUrl">Base URL</param>
        /// <param name="param">Batch count parameters</param>
        /// <returns>Complete URL with query string</returns>
        public static string CreateBatchUrl(string baseUrl, BatchCountParam param)
        {
            var parameters = new Dictionary<string, object>
            {
                ["batchCount"] = param.BatchCount,
                ["fromDate"] = param.FromDate,
                ["toDate"] = param.ToDate,
                ["sq"] = param.SQ
            };

            var queryString = CreateQueryString(parameters);
            return $"{baseUrl}?{queryString}";
        }

        /// <summary>
        /// Validates API response structure
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <param name="expectedStatusCode">Expected status code</param>
        /// <returns>True if response is valid</returns>
        public static async Task<bool> ValidateApiResponse(
            HttpResponseMessage response,
            System.Net.HttpStatusCode expectedStatusCode = System.Net.HttpStatusCode.OK)
        {
            if (response.StatusCode != expectedStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
                return false;

            // Basic JSON validation
            try
            {
                JsonDocument.Parse(content);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates test service scope for dependency injection testing
        /// </summary>
        /// <param name="factory">Web application factory</param>
        /// <returns>Service scope</returns>
        public static IServiceScope CreateTestScope(WebApplicationFactory<Aff12B.Ext.API.Site.Startup> factory)
        {
            return factory.Services.CreateScope();
        }

        /// <summary>
        /// Gets service from test scope
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="scope">Service scope</param>
        /// <returns>Service instance</returns>
        public static T GetService<T>(IServiceScope scope) where T : notnull
        {
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}
