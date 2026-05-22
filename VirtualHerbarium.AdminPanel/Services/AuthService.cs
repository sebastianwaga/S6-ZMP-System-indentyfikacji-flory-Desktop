using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class AuthService
    {
        public static AuthService Instance { get; } = new AuthService();

        private readonly HttpClient _http;

        public string? Token { get; private set; }
        public LoginResponse? CurrentUser { get; private set; }

        private int _failedAttempts = 0;
        private bool _isLocked = false;
        private DateTime _lockUntil;

        private AuthService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://ezielnik-production.up.railway.app"),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        private string EncryptToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        private string DecryptToken(string encrypted)
        {
            var bytes = Convert.FromBase64String(encrypted);
            var decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }

        public async Task<LoginResponse?> LoginAsync(string login, string password)
        {
            if (_isLocked && DateTime.Now < _lockUntil)
                return new LoginResponse { message = "LOCKED" };

            var request = new LoginRequest
            {
                login = login,
                password = password
            };

            HttpResponseMessage response;

            try
            {
                response = await _http.PostAsJsonAsync("/users/login", request);
            }
            catch
            {
                return new LoginResponse { message = "NETWORK_ERROR" };
            }

            if (!response.IsSuccessStatusCode)
            {
                _failedAttempts++;

                if (_failedAttempts >= 3)
                {
                    _isLocked = true;
                    _lockUntil = DateTime.Now.AddSeconds(10);
                }

                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (data != null)
            {
                _failedAttempts = 0;
                _isLocked = false;

                Token = data.token;
                CurrentUser = data;

                if (Token != null)
                {
                    _http.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

                    var encrypted = EncryptToken(Token);
                }
            }

            return data;
        }

        public void Logout()
        {
            Token = null;
            CurrentUser = null;
            _http.DefaultRequestHeaders.Authorization = null;
        }
    }
}
