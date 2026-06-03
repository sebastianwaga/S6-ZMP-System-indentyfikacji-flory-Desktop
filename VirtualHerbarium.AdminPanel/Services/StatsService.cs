using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class StatsService
    {
        public static StatsService Instance { get; } = new StatsService();

        private readonly HttpClient _http;

        private StatsService()
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

        public async Task<ApiResult<StatsOverviewResponse>> GetOverviewAsync()
        {
            try
            {
                var response = await _http.GetAsync("/stats/overview");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResult<StatsOverviewResponse>
                    {
                        Success = false,
                        Error = await response.Content.ReadAsStringAsync(),
                        StatusCode = (int)response.StatusCode
                    };
                }

                var data = await response.Content.ReadFromJsonAsync<StatsOverviewResponse>();

                return new ApiResult<StatsOverviewResponse>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<StatsOverviewResponse>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }
    }
}
