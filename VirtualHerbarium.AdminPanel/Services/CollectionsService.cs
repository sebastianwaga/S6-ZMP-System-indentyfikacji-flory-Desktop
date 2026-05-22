using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class CollectionsService
    {
        private readonly HttpClient _http;

        public CollectionsService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://ezielnik-production.up.railway.app")
            };

            if (!string.IsNullOrEmpty(AuthService.Instance.Token))
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

        public async Task<ApiResult<List<HerbariumStatsResponse>>> GetCollectionsAsync()
        {
            try
            {
                var response = await _http.GetAsync("/stats/herbaria");

                var raw = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResult<List<HerbariumStatsResponse>>
                    {
                        Success = false,
                        Error = raw,
                        StatusCode = (int)response.StatusCode
                    };
                }

                var data = await response.Content.ReadFromJsonAsync<List<HerbariumStatsResponse>>();

                return new ApiResult<List<HerbariumStatsResponse>>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<HerbariumStatsResponse>>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> DeleteCollectionAsync(string userId, string herbariumId)
        {
            try
            {
                var response = await _http.DeleteAsync($"/users/{userId}/herbaria/{herbariumId}");

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
        public async Task<ApiResult<HerbariumDetailsResponse>> GetHerbariumDetailsAsync(string id)
        {
            try
            {
                var response = await _http.GetAsync($"/herbaria/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<HerbariumDetailsResponse>();
                    return new ApiResult<HerbariumDetailsResponse> { Success = true, Data = data };
                }

                return HandleError<HerbariumDetailsResponse>(response);
            }
            catch
            {
                return new ApiResult<HerbariumDetailsResponse>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }

        public async Task<ApiResult<List<PlantResponse>>> GetHerbariumPlantsAsync(string id)
        {
            try
            {
                var response = await _http.GetAsync($"/herbaria/{id}/plants");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<PlantResponse>>();
                    return new ApiResult<List<PlantResponse>> { Success = true, Data = data };
                }

                return HandleError<List<PlantResponse>>(response);
            }
            catch
            {
                return new ApiResult<List<PlantResponse>>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }
    }
}



