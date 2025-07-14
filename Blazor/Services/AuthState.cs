using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Blazor.Services
{
    /// <summary>
    /// AuthState holder styr på brugerens login-status og info i hele appen.
    /// Bruges som singleton via Dependency Injection.
    /// Nu med krypteret localStorage support og JWT.
    /// </summary>
    public class AuthState
    {
        /// <summary>
        /// Event der trigges når login-status eller brugerdata ændres.
        /// Komponenter kan subscribe for at opdatere UI automatisk.
        /// </summary>
        public event Action? OnChange;

        /// <summary>
        /// Er brugeren logget ind?
        /// </summary>
        public bool IsLoggedIn { get; private set; } = false;

        /// <summary>
        /// Brugernavn eller e-mail på den aktuelle bruger.
        /// </summary>
        public string? UserName { get; private set; }

        /// <summary>
        /// Liste over brugerens roller (kan bruges til adgangskontrol).
        /// </summary>
        public List<string> Roles { get; private set; } = new();

        /// <summary>
        /// JWT token for den aktuelle bruger.
        /// </summary>
        public string? JWTToken { get; private set; }

        /// <summary>
        /// JWT payload (dekodet token data).
        /// </summary>
        public JWTPayload? JWTPayload { get; private set; }

        private const string StorageKey = "authstate";
        private readonly IJSRuntime? _js;

        // Ctor til DI (JSRuntime kun nødvendig i Blazor WASM)
        public AuthState() { }
        public AuthState(IJSRuntime js) { _js = js; }

        /// <summary>
        /// Log brugeren ind og sæt brugerdata. Gemmer state krypteret i localStorage.
        /// </summary>
        public async Task LoginAsync(string userName, IEnumerable<string>? roles = null)
        {
            IsLoggedIn = true;
            UserName = userName;
            Roles = roles != null ? new List<string>(roles) : new List<string>();
            await SaveAsync();
            NotifyStateChanged();
        }

        /// <summary>
        /// Log brugeren ind med JWT token.
        /// </summary>
        public async Task LoginWithJWTAsync(string token)
        {
            if (_js == null) return;

            var module = await _js.InvokeAsync<IJSObjectReference>("import", "./js/secureStorage.js");
            
            // Gem JWT token krypteret
            await module.InvokeVoidAsync("setJWTToken", token);
            
            // Dekod JWT payload
            var payload = await module.InvokeAsync<JWTPayload>("decodeJWT", token);
            
            JWTToken = token;
            JWTPayload = payload;
            IsLoggedIn = true;
            UserName = payload?.Name ?? payload?.Email ?? "Ukendt bruger";
            Roles = new List<string>(); // JWT har ikke roller i vores setup
            
            Console.WriteLine($"🔐 JWT Login successful:");
            Console.WriteLine($"Token length: {token.Length}");
            Console.WriteLine($"User: {UserName}");
            Console.WriteLine($"Payload: {payload?.Name} ({payload?.Email})");
            
            await SaveAsync();
            NotifyStateChanged();
        }

        /// <summary>
        /// Log brugeren ud og nulstil state. Fjerner state fra localStorage.
        /// </summary>
        public async Task LogoutAsync()
        {
            IsLoggedIn = false;
            UserName = null;
            Roles.Clear();
            JWTToken = null;
            JWTPayload = null;
            await RemoveAsync();
            await RemoveJWTAsync();
            NotifyStateChanged();
        }

        /// <summary>
        /// Gemmer state krypteret i localStorage via JS interop (kryptering sker i JS).
        /// </summary>
        public async Task SaveAsync()
        {
            if (_js == null) return;
            var state = JsonSerializer.Serialize(new AuthStateDto
            {
                IsLoggedIn = this.IsLoggedIn,
                UserName = this.UserName,
                Roles = this.Roles
            });
            var module = await _js.InvokeAsync<IJSObjectReference>("import", "./js/secureStorage.js");
            // Brug JS til at kryptere og gemme
            await module.InvokeVoidAsync("setEncryptedItem", StorageKey, state);
        }

        /// <summary>
        /// Hent state fra localStorage, dekrypter og sæt properties. Kald ved app-start.
        /// </summary>
        public async Task LoadAsync(IJSRuntime js)
        {
            var module = await js.InvokeAsync<IJSObjectReference>("import", "./js/secureStorage.js");
            
            // Hent normal state
            var json = await module.InvokeAsync<string>("getDecryptedItem", StorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var dto = JsonSerializer.Deserialize<AuthStateDto>(json);
                    if (dto != null)
                    {
                        IsLoggedIn = dto.IsLoggedIn;
                        UserName = dto.UserName;
                        Roles = dto.Roles ?? new List<string>();
                    }
                }
                catch { /* Ignorer fejl, fx hvis nøglen er ændret */ }
            }

            // Hent JWT token
            var token = await module.InvokeAsync<string>("getJWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var payload = await module.InvokeAsync<JWTPayload>("decodeJWT", token);
                    if (payload != null)
                    {
                        JWTToken = token;
                        JWTPayload = payload;
                        IsLoggedIn = true;
                        UserName = payload.Name ?? payload.Email ?? "Ukendt bruger";
                        
                        Console.WriteLine($"🔐 JWT Token loaded from storage:");
                        Console.WriteLine($"Token length: {token.Length}");
                        Console.WriteLine($"User: {UserName}");
                        Console.WriteLine($"Payload: {payload.Name} ({payload.Email})");
                    }
                }
                catch { /* Ignorer fejl */ }
            }
            else
            {
                Console.WriteLine("⚠️ No JWT token found in storage");
            }
            
            NotifyStateChanged();
        }

        /// <summary>
        /// Fjern state fra localStorage.
        /// </summary>
        public async Task RemoveAsync()
        {
            if (_js == null) return;
            var module = await _js.InvokeAsync<IJSObjectReference>("import", "./js/secureStorage.js");
            await module.InvokeVoidAsync("removeItem", StorageKey);
        }

        /// <summary>
        /// Fjern JWT token fra localStorage.
        /// </summary>
        public async Task RemoveJWTAsync()
        {
            if (_js == null) return;
            var module = await _js.InvokeAsync<IJSObjectReference>("import", "./js/secureStorage.js");
            await module.InvokeVoidAsync("removeJWTToken");
        }

        /// <summary>
        /// Intern metode til at notificere subscribers om ændringer.
        /// </summary>
        private void NotifyStateChanged() => OnChange?.Invoke();

        // DTO til serialisering (ingen events/JS osv.)
        private class AuthStateDto
        {
            public bool IsLoggedIn { get; set; }
            public string? UserName { get; set; }
            public List<string>? Roles { get; set; }
        }
    }

    public class JWTPayload
    {
        [JsonPropertyName("nameid")]
        public string? Id { get; set; }
        
        [JsonPropertyName("unique_name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        
        [JsonPropertyName("exp")]
        public long Exp { get; set; }
        
        [JsonPropertyName("iat")]
        public long Iat { get; set; }
        
        [JsonPropertyName("nbf")]
        public long Nbf { get; set; }
        
        [JsonPropertyName("iss")]
        public string? Issuer { get; set; }
        
        [JsonPropertyName("aud")]
        public string? Audience { get; set; }
    }
} 