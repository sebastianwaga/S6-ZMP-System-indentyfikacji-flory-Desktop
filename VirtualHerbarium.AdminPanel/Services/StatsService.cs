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

        private StatsService() { }

        public async Task<ApiResult<StatsOverviewResponse>> GetOverviewAsync()
        {
            try
            {
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync("stats/overview")
                );

                if (response == null)
                {
                    return new ApiResult<StatsOverviewResponse>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

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
