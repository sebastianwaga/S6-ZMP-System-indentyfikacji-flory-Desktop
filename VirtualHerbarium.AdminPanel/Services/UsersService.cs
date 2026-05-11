using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class UsersService
    {
        private readonly HttpClient _http;

        public UsersService(string token)
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080")
            };

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<ApiResult<List<AdminUserResponse>>> GetUsersAsync()
        {
            try
            {
                var response = await _http.GetAsync("/users");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<AdminUserResponse>>();
                    return new ApiResult<List<AdminUserResponse>> { Success = true, Data = data };
                }

                return new ApiResult<List<AdminUserResponse>>
                {
                    Success = false,
                    Error = await response.Content.ReadAsStringAsync(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<AdminUserResponse>>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> BanUserAsync(string id)
        {
            try
            {
                var response = await _http.PatchAsync($"/users/{id}/ban", null);

                if (response.IsSuccessStatusCode)
                    return new ApiResult<bool> { Success = true, Data = true };

                return new ApiResult<bool>
                {
                    Success = false,
                    Error = await response.Content.ReadAsStringAsync(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> UnbanUserAsync(string id)
        {
            try
            {
                var response = await _http.PatchAsync($"/users/{id}/unban", null);

                if (response.IsSuccessStatusCode)
                    return new ApiResult<bool> { Success = true, Data = true };

                return new ApiResult<bool>
                {
                    Success = false,
                    Error = await response.Content.ReadAsStringAsync(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> MakeAdminAsync(string id)
        {
            try
            {
                var response = await _http.PatchAsync($"/users/{id}/make-admin", null);

                if (response.IsSuccessStatusCode)
                    return new ApiResult<bool> { Success = true, Data = true };

                return new ApiResult<bool>
                {
                    Success = false,
                    Error = await response.Content.ReadAsStringAsync(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> RemoveAdminAsync(string id)
        {
            try
            {
                var response = await _http.PatchAsync($"/users/{id}/remove-admin", null);

                if (response.IsSuccessStatusCode)
                    return new ApiResult<bool> { Success = true, Data = true };

                return new ApiResult<bool>
                {
                    Success = false,
                    Error = await response.Content.ReadAsStringAsync(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> SendWarningAsync(string id, string message)
        {
            try
            {
                var body = new { message };
                var response = await _http.PostAsJsonAsync($"/users/{id}/warning", body);

                if (response.IsSuccessStatusCode)
                    return new ApiResult<bool> { Success = true, Data = true };

                return new ApiResult<bool>
                {
                    Success = false,
                    Error = await response.Content.ReadAsStringAsync(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }
    }
}
