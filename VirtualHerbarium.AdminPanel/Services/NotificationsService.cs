using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class NotificationsService
    {
        public static NotificationsService Instance { get; } = new NotificationsService();

        private NotificationsService() { }

        public async Task<List<NotificationResponse>?> GetNotificationsAsync()
        {
            var response = await AuthService.Instance.SendAuthorizedAsync(
                http => http.GetAsync("notifications")
            );

            if (response == null || !response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<NotificationResponse>>();
        }

        public async Task<List<NotificationResponse>?> GetUnreadAsync()
        {
            var response = await AuthService.Instance.SendAuthorizedAsync(
                http => http.GetAsync("notifications/unread")
            );

            if (response == null || !response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<NotificationResponse>>();
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            var response = await AuthService.Instance.SendAuthorizedAsync(
                http => http.PatchAsync($"notifications/{id}/read", null)
            );

            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> MarkAllAsReadAsync()
        {
            var response = await AuthService.Instance.SendAuthorizedAsync(
                http => http.PatchAsync("notifications/read-all", null)
            );

            return response != null && response.IsSuccessStatusCode;
        }
    }
}
