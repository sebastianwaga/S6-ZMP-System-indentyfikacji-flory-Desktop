using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class PlantsService
    {
        public static PlantsService Instance { get; } = new PlantsService();

        private readonly HttpClient _http;

        private PlantsService()
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
        public async Task<PlantPhotoResponse?> GetPhotoMetadataAsync(string herbariumId, string plantId, string photoId)
        {
            try
            {
                var response = await _http.GetAsync(
                    $"/herbaria/{herbariumId}/plants/{plantId}/photos/{photoId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<PlantPhotoResponse>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<BitmapImage?> LoadPhotoAsync(string relativeUrl)
        {
            try
            {
                var clean = relativeUrl.TrimStart('/');
                var url = $"{_http.BaseAddress}{clean}";

                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var bytes = await response.Content.ReadAsByteArrayAsync();
                if (bytes.Length == 0)
                    return null;

                using var ms = new MemoryStream(bytes);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();

                return image;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ApiResult<List<PlantResponse>>> GetPlantsAsync()
        {
            try
            {
                var herbariaResponse = await _http.GetAsync("/stats/herbaria");

                if (!herbariaResponse.IsSuccessStatusCode)
                {
                    return new ApiResult<List<PlantResponse>>
                    {
                        Success = false,
                        Error = await herbariaResponse.Content.ReadAsStringAsync(),
                        StatusCode = (int)herbariaResponse.StatusCode
                    };
                }

                var herbaria = await herbariaResponse.Content.ReadFromJsonAsync<List<HerbariumStatsResponse>>();

                var allPlants = new List<PlantResponse>();

                foreach (var herbarium in herbaria)
                {
                    var plantsResponse = await _http.GetAsync($"/herbaria/{herbarium.id}/plants");

                    if (!plantsResponse.IsSuccessStatusCode)
                        continue;

                    var plants = await plantsResponse.Content.ReadFromJsonAsync<List<PlantResponse>>();

                    if (plants != null)
                    {
                        foreach (var p in plants)
                            p.herbariumId = herbarium.id;

                        allPlants.AddRange(plants);
                    }
                }

                return new ApiResult<List<PlantResponse>>
                {
                    Success = true,
                    Data = allPlants
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<PlantResponse>>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }
        public async Task<ApiResult<PlantDetailsResponse>> GetPlantDetailsAsync(string herbariumId, string plantId)
        {
            try
            {
                var response = await _http.GetAsync($"/herbaria/{herbariumId}/plants/{plantId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResult<PlantDetailsResponse>
                    {
                        Success = false,
                        StatusCode = (int)response.StatusCode
                    };
                }

                var data = await response.Content.ReadFromJsonAsync<PlantDetailsResponse>();

                return new ApiResult<PlantDetailsResponse>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<PlantDetailsResponse>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> DeletePlantAsync(string herbariumId, string plantId)
        {
            try
            {
                var response = await _http.DeleteAsync($"/herbaria/{herbariumId}/plants/{plantId}");

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
