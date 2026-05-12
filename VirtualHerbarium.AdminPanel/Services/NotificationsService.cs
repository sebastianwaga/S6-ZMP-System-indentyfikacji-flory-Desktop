using System;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class NotificationsService
    {
        private readonly bool _useMock = true;

        public async Task<ApiResult<bool>> SendNotificationAsync(NotificationRequest request)
        {
            if (_useMock)
            {
                await Task.Delay(300);
                return new ApiResult<bool>
                {
                    Success = true,
                    Data = true
                };
            }

            try
            {
                throw new NotImplementedException("API jeszcze nie działa.");
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
