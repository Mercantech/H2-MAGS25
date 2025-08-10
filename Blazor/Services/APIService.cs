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

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("User/login", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponse>();
                }
            }
            catch
            {
                // Ignorer fejl
            }
            return null;
        }

        public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("User/register", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RegisterResponse>();
                }
            }
            catch
            {
                // Ignorer fejl
            }
            return null;
        }

        public async Task<SearchResult?> GetUsersWithOptionsAsync(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            bool includeBookings = false,
            int? limit = null,
            string sortBy = "name",
            bool ascending = true)
        {
            try
            {
                var queryParams = new List<string>();
                
                if (page > 1) queryParams.Add($"page={page}");
                if (pageSize != 10) queryParams.Add($"pageSize={pageSize}");
                if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
                if (includeBookings) queryParams.Add("includeBookings=true");
                if (limit.HasValue) queryParams.Add($"limit={limit.Value}");
                if (sortBy != "name") queryParams.Add($"sortBy={sortBy}");
                if (!ascending) queryParams.Add("ascending=false");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var url = $"User/search{queryString}";

                var request = CreateAuthenticatedRequest(HttpMethod.Get, url);
                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SearchResult>();
                }
            }
            catch
            {
                // Ignorer fejl
            }
            return null;
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    public class RegisterResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    // Data klasser for search result
    public class SearchResult
    {
        public List<User> Data { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
        public SearchOptions Options { get; set; } = new();
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class SearchOptions
    {
        public string? Search { get; set; }
        public bool IncludeBookings { get; set; }
        public int? Limit { get; set; }
        public string SortBy { get; set; } = "";
        public bool Ascending { get; set; }
    }

    public class User
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<BookingUser>? BookingUsers { get; set; }
    }

    public class BookingUser
    {
        public string Id { get; set; } = "";
        public string UserId { get; set; } = "";
        public string BookingId { get; set; } = "";
        public Booking? Booking { get; set; }
    }

    public class Booking
    {
        public string Id { get; set; } = "";
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
