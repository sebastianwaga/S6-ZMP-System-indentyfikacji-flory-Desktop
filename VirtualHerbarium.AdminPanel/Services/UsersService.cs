using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class UsersService
    {
        private readonly HttpClient _http;

        public UsersService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://ezielnik-production.up.railway.app"),
                Timeout = TimeSpan.FromSeconds(10)
            };

            if (AuthService.Instance.Token != null)
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthService.Instance.Token);
            }
        }

        private ApiResult<T> HandleError<T>(HttpResponseMessage response)
        {
            int code = (int)response.StatusCode;

            return new ApiResult<T>
            {
                Success = false,
                StatusCode = code,
                Error = code switch
                {
                    401 => AppResources.Error_SessionExpired,
                    403 => AppResources.Error_NoPermission,
                    404 => AppResources.Error_UserNotFound,
                    _ => AppResources.Error_Server
                }
            };
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

                return HandleError<List<AdminUserResponse>>(response);
            }
            catch
            {
                return new ApiResult<List<AdminUserResponse>>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }

        public async Task<ApiResult<UserDetailsResponse>> GetUserDetailsAsync(string id)
        {
            try
            {
                var response = await _http.GetAsync($"/stats/users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<UserDetailsResponse>();
                    return new ApiResult<UserDetailsResponse> { Success = true, Data = data };
                }

                return HandleError<UserDetailsResponse>(response);
            }
            catch
            {
                return new ApiResult<UserDetailsResponse>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }

        public async Task<ApiResult<UserFriendsResponse>> GetUserFriendsAsync(string id)
        {
            try
            {
                var response = await _http.GetAsync($"/stats/users/{id}/friends");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<UserFriendsResponse>();
                    return new ApiResult<UserFriendsResponse> { Success = true, Data = data };
                }

                return HandleError<UserFriendsResponse>(response);
            }
            catch
            {
                return new ApiResult<UserFriendsResponse>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }

        public async Task<ApiResult<bool>> BanUserAsync(string id)
        {
            try
            {
                var response = await _http.PatchAsync($"/users/{id}/ban", null);
                return response.IsSuccessStatusCode
                    ? new ApiResult<bool> { Success = true, Data = true }
                    : HandleError<bool>(response);
            }
            catch
            {
                return new ApiResult<bool> { Success = false, Error = AppResources.Error_Server };
            }
        }

        public async Task<ApiResult<bool>> UnbanUserAsync(string id)
        {
            try
            {
                var response = await _http.PatchAsync($"/users/{id}/unban", null);
                return response.IsSuccessStatusCode
                    ? new ApiResult<bool> { Success = true, Data = true }
                    : HandleError<bool>(response);
            }
            catch
            {
                return new ApiResult<bool> { Success = false, Error = AppResources.Error_Server };
            }
        }

        public async Task<ApiResult<bool>> MakeAdminAsync(string id)
        {
            try
            {
                var response = await _http.PatchAsync($"/users/{id}/make-admin", null);
                return response.IsSuccessStatusCode
                    ? new ApiResult<bool> { Success = true, Data = true }
                    : HandleError<bool>(response);
            }
            catch
            {
                return new ApiResult<bool> { Success = false, Error = AppResources.Error_Server };
            }
        }

        public async Task<ApiResult<bool>> RemoveAdminAsync(string id)
        {
            try
            {
                var response = await _http.PatchAsync($"/users/{id}/remove-admin", null);
                return response.IsSuccessStatusCode
                    ? new ApiResult<bool> { Success = true, Data = true }
                    : HandleError<bool>(response);
            }
            catch
            {
                return new ApiResult<bool> { Success = false, Error = AppResources.Error_Server };
            }
        }

        public async Task<bool> SendWarningAsync(string userId, string subject, string message)
        {
            var body = new { subject, message };

            var response = await _http.PostAsJsonAsync($"/users/{userId}/warning", body);

            if (!response.IsSuccessStatusCode)
                return false;

            var text = await response.Content.ReadAsStringAsync();
            return text.Contains("success", StringComparison.OrdinalIgnoreCase);
        }
    }
}
