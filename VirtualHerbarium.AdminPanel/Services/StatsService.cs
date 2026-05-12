using System;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class StatsService
    {
        private readonly bool _useMock = true;

        public async Task<ApiResult<StatsResponse>> GetStatsAsync()
        {
            if (_useMock)
            {
                await Task.Delay(200);

                return new ApiResult<StatsResponse>
                {
                    Success = true,
                    Data = new StatsResponse
                    {
                        users = 42,
                        plants = 128,
                        collections = 17
                    }
                };
            }

            try
            {
                throw new NotImplementedException("API jeszcze nie działa.");
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
