using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class NotificationsService
    {
        private readonly HttpClient _http;

        public NotificationsService()
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

        public async Task<List<NotificationResponse>?> GetNotificationsAsync()
        {
            var response = await _http.GetAsync("/notifications");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<NotificationResponse>>();
        }

        public async Task<List<NotificationResponse>?> GetUnreadAsync()
        {
            var response = await _http.GetAsync("/notifications/unread");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<NotificationResponse>>();
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            var response = await _http.PatchAsync($"/notifications/{id}/read", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> MarkAllAsReadAsync()
        {
            var response = await _http.PatchAsync("/notifications/read-all", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendNotificationAsync(NotificationRequest request)
        {
            var response = await _http.PostAsJsonAsync("/admin/notifications", request);
            return response.IsSuccessStatusCode;
        }
    }
}
