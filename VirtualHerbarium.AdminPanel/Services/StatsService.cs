using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class StatsService
    {
        private readonly HttpClient _http;

        public StatsService()
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

        private class OverviewDto
        {
            public int totalUsers { get; set; }
            public int totalHerbaria { get; set; }
            public int totalPlants { get; set; }
        }

        public async Task<ApiResult<StatsResponse>> GetStatsAsync()
        {
            try
            {
                var response = await _http.GetAsync("/stats/overview");
                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResult<StatsResponse>
                    {
                        Success = false,
                        Error = await response.Content.ReadAsStringAsync(),
                        StatusCode = (int)response.StatusCode
                    };
                }

                var dto = await response.Content.ReadFromJsonAsync<OverviewDto>();
                if (dto == null)
                {
                    return new ApiResult<StatsResponse>
                    {
                        Success = false,
                        Error = "Brak danych z /stats/overview",
                        StatusCode = 0
                    };
                }

                return new ApiResult<StatsResponse>
                {
                    Success = true,
                    Data = new StatsResponse
                    {
                        users = dto.totalUsers,
                        plants = dto.totalPlants,
                        collections = dto.totalHerbaria
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<StatsResponse>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }
    }
}
