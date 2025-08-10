using System.Net.Http.Json;
using System.Text.Json;

namespace Blazor.Services
{
    public partial class APIService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthState _authState;

        public APIService(HttpClient httpClient, AuthState authState)
        {
            _httpClient = httpClient;
            _authState = authState;
        }

        private HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            
            // Tilføj JWT token hvis brugeren er logget ind
            if (_authState.IsLoggedIn && !string.IsNullOrEmpty(_authState.JWTToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authState.JWTToken);
                Console.WriteLine($"🔐 Adding JWT token to request: {url}");
                Console.WriteLine($"Token: {_authState.JWTToken.Substring(0, Math.Min(50, _authState.JWTToken.Length))}...");
            }
            else
            {
                Console.WriteLine($"⚠️ No JWT token available for request: {url}");
                Console.WriteLine($"IsLoggedIn: {_authState.IsLoggedIn}");
                Console.WriteLine($"JWTToken null/empty: {string.IsNullOrEmpty(_authState.JWTToken)}");
            }
            
            return request;
        }


    }


}
