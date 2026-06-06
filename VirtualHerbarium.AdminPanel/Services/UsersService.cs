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
        public static UsersService Instance { get; } = new UsersService();

        private UsersService() { }

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
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync("stats/users")
                );

                if (response == null)
                    return ApiResult<List<AdminUserResponse>>.Unauthorized();

                if (!response.IsSuccessStatusCode)
                    return HandleError<List<AdminUserResponse>>(response);

                var data = await response.Content.ReadFromJsonAsync<List<AdminUserResponse>>();

                return new ApiResult<List<AdminUserResponse>>
                {
                    Success = true,
                    Data = data
                };
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
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync($"stats/users/{id}")
                );

                if (response == null)
                    return ApiResult<UserDetailsResponse>.Unauthorized();

                if (!response.IsSuccessStatusCode)
                    return HandleError<UserDetailsResponse>(response);

                var data = await response.Content.ReadFromJsonAsync<UserDetailsResponse>();

                return new ApiResult<UserDetailsResponse>
                {
                    Success = true,
                    Data = data
                };
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
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync($"stats/users/{id}/friends")
                );

                if (response == null)
                    return ApiResult<UserFriendsResponse>.Unauthorized();

                if (!response.IsSuccessStatusCode)
                    return HandleError<UserFriendsResponse>(response);

                var data = await response.Content.ReadFromJsonAsync<UserFriendsResponse>();

                return new ApiResult<UserFriendsResponse>
                {
                    Success = true,
                    Data = data
                };
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
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PatchAsync($"users/{id}/ban", null)
                );

                if (response == null)
                    return ApiResult<bool>.Unauthorized();

                return response.IsSuccessStatusCode
                    ? ApiResult<bool>.SuccessTrue()
                    : HandleError<bool>(response);
            }
            catch
            {
                return ApiResult<bool>.ServerError();
            }
        }

        public async Task<ApiResult<bool>> UnbanUserAsync(string id)
        {
            try
            {
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PatchAsync($"users/{id}/unban", null)
                );

                if (response == null)
                    return ApiResult<bool>.Unauthorized();

                return response.IsSuccessStatusCode
                    ? ApiResult<bool>.SuccessTrue()
                    : HandleError<bool>(response);
            }
            catch
            {
                return ApiResult<bool>.ServerError();
            }
        }

        public async Task<ApiResult<bool>> MakeAdminAsync(string id)
        {
            try
            {
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PatchAsync($"users/{id}/make-admin", null)
                );

                if (response == null)
                    return ApiResult<bool>.Unauthorized();

                return response.IsSuccessStatusCode
                    ? ApiResult<bool>.SuccessTrue()
                    : HandleError<bool>(response);
            }
            catch
            {
                return ApiResult<bool>.ServerError();
            }
        }

        public async Task<ApiResult<bool>> RemoveAdminAsync(string id)
        {
            try
            {
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PatchAsync($"users/{id}/remove-admin", null)
                );

                if (response == null)
                    return ApiResult<bool>.Unauthorized();

                return response.IsSuccessStatusCode
                    ? ApiResult<bool>.SuccessTrue()
                    : HandleError<bool>(response);
            }
            catch
            {
                return ApiResult<bool>.ServerError();
            }
        }

        public async Task<bool> SendWarningAsync(string userId, string subject, string message)
        {
            var body = new { subject, message };

            var response = await AuthService.Instance.SendAuthorizedAsync(
                http => http.PostAsJsonAsync($"users/{userId}/warning", body)
            );

            if (response == null || !response.IsSuccessStatusCode)
                return false;

            var text = await response.Content.ReadAsStringAsync();
            return text.Contains("success", StringComparison.OrdinalIgnoreCase);
        }
        public async Task<ApiResult<bool>> DeleteUserAsync(string id)
        {
            try
            {
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.DeleteAsync($"users/{id}")
                );

                if (response == null)
                    return ApiResult<bool>.Unauthorized();

                return response.IsSuccessStatusCode
                    ? ApiResult<bool>.SuccessTrue()
                    : HandleError<bool>(response);
            }
            catch
            {
                return ApiResult<bool>.ServerError();
            }
        }
    }
}
