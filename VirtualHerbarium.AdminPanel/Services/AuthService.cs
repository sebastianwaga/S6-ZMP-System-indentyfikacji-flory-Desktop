using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        public string? Token { get; private set; }
        public LoginResponse? CurrentUser { get; private set; }

        public AuthService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080")
            };
        }

        public async Task<LoginResponse?> LoginAsync(string login, string password)
        {
            var request = new LoginRequest
            {
                login = login,
                password = password
            };

            var response = await _http.PostAsJsonAsync("/users/login", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (data != null)
            {
                Token = data.token;
                CurrentUser = data;

                if (Token != null)
                {
                    _http.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                }
            }

            return data;
        }
    }
}
