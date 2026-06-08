using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services.Offline;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class SyncWorker
    {
        public static SyncWorker Instance { get; } = new SyncWorker();

        private readonly DispatcherTimer _timer;
        private readonly HttpClient _http;

        private SyncWorker()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://ezielnik-production.up.railway.app/")
            };

            if (!string.IsNullOrEmpty(AuthService.Instance.Token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthService.Instance.Token);
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            _timer.Tick += async (_, __) => await TickAsync();
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        private async Task TickAsync()
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    AuthService.Instance.Token);

            if (!InternetService.Instance.IsOnline)
                return;

            await ProcessQueueAsync();
            await PullServerChangesAsync();
        }

        private async Task ProcessQueueAsync()
        {
            var pending = LocalDatabaseService.Instance.GetPendingQueueItems();

            foreach (var item in pending)
            {
                bool success = item.ActionType switch
                {
                    "create" => await HandleCreateAsync(item),
                    "update" => await HandleUpdateAsync(item),
                    "delete" => await HandleDeleteAsync(item),
                    "move" => await HandleMoveAsync(item),
                    _ => false
                };

                if (success)
                    LocalDatabaseService.Instance.MarkQueueItemAsSent(item.Id);
                else
                    LocalDatabaseService.Instance.IncrementRetry(item.Id);
            }
        }

        private class PlantContext
        {
            public string herbariumId { get; set; }
            public string plantId { get; set; }
            public string name { get; set; }
        }

        private class PhotoContext
        {
            public string herbariumId { get; set; }
            public string plantId { get; set; }
            public string description { get; set; }
        }

        private class PhotoMoveContext
        {
            public string herbariumId { get; set; }
            public string plantId { get; set; }
            public string targetPlantId { get; set; }
        }

        private async Task<bool> HandleCreateAsync(SyncQueueItem item)
        {
            var json = item.Payload;

            return item.EntityType switch
            {
                "herbarium" =>
                    await SendAsync("herbaria", json, HttpMethod.Post),

                "plant" =>
                    await HandleCreatePlantAsync(json),

                "photo" =>
                    false, 

                _ => false
            };
        }

        private async Task<bool> HandleCreatePlantAsync(string json)
        {
            var ctx = JsonSerializer.Deserialize<PlantContext>(json);
            if (ctx == null || string.IsNullOrEmpty(ctx.herbariumId))
                return false;

            return await SendAsync(
                $"herbaria/{ctx.herbariumId}/plants/add",
                json,
                HttpMethod.Post);
        }

        private async Task<bool> HandleUpdateAsync(SyncQueueItem item)
        {
            var json = item.Payload;

            return item.EntityType switch
            {
                "herbarium" =>
                    await SendAsync($"herbaria/{item.EntityId}", json, HttpMethod.Patch),

                "plant" =>
                    await HandleUpdatePlantAsync(item.EntityId, json),

                "photo" =>
                    await HandleUpdatePhotoAsync(item.EntityId, json),

                _ => false
            };
        }

        private async Task<bool> HandleUpdatePlantAsync(string plantId, string json)
        {
            var ctx = JsonSerializer.Deserialize<PlantContext>(json);
            if (ctx == null || string.IsNullOrEmpty(ctx.herbariumId))
                return false;

            return await SendAsync(
                $"herbaria/{ctx.herbariumId}/plants/{plantId}",
                json,
                HttpMethod.Patch);
        }

        private async Task<bool> HandleUpdatePhotoAsync(string photoId, string json)
        {
            var ctx = JsonSerializer.Deserialize<PhotoContext>(json);
            if (ctx == null || string.IsNullOrEmpty(ctx.herbariumId) || string.IsNullOrEmpty(ctx.plantId))
                return false;

            return await SendAsync(
                $"herbaria/{ctx.herbariumId}/plants/{ctx.plantId}/photos/{photoId}",
                json,
                HttpMethod.Patch);
        }

        private async Task<bool> HandleDeleteAsync(SyncQueueItem item)
        {
            var json = item.Payload;

            return item.EntityType switch
            {
                "herbarium" =>
                    await SendAsync($"herbaria/{item.EntityId}", null, HttpMethod.Delete),

                "plant" =>
                    await HandleDeletePlantAsync(item.EntityId, json),

                "photo" =>
                    await HandleDeletePhotoAsync(item.EntityId, json),

                _ => false
            };
        }

        private async Task<bool> HandleDeletePlantAsync(string plantId, string json)
        {
            var ctx = JsonSerializer.Deserialize<PlantContext>(json);
            if (ctx == null || string.IsNullOrEmpty(ctx.herbariumId))
                return false;

            return await SendAsync(
                $"herbaria/{ctx.herbariumId}/plants/{plantId}",
                null,
                HttpMethod.Delete);
        }

        private async Task<bool> HandleDeletePhotoAsync(string photoId, string json)
        {
            var ctx = JsonSerializer.Deserialize<PhotoContext>(json);
            if (ctx == null || string.IsNullOrEmpty(ctx.herbariumId) || string.IsNullOrEmpty(ctx.plantId))
                return false;

            return await SendAsync(
                $"herbaria/{ctx.herbariumId}/plants/{ctx.plantId}/photos/{photoId}",
                null,
                HttpMethod.Delete);
        }

        private async Task<bool> HandleMoveAsync(SyncQueueItem item)
        {
            var ctx = JsonSerializer.Deserialize<PhotoMoveContext>(item.Payload);
            if (ctx == null)
                return false;

            return await SendAsync(
                $"herbaria/{ctx.herbariumId}/plants/{ctx.plantId}/photos/{item.EntityId}/move",
                item.Payload,
                HttpMethod.Post);
        }

        private async Task<bool> SendAsync(string url, string json, HttpMethod method)
        {
            try
            {
                HttpResponseMessage response;

                if (method == HttpMethod.Post || method == HttpMethod.Patch)
                {
                    var content = new StringContent(
                        json ?? "{}",
                        Encoding.UTF8,
                        "application/json");

                    response = method == HttpMethod.Post
                        ? await _http.PostAsync(url, content)
                        : await _http.PatchAsync(url, content);
                }
                else if (method == HttpMethod.Delete)
                {
                    response = await _http.DeleteAsync(url);
                }
                else
                {
                    return false;
                }

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task PullServerChangesAsync()
        {
            try
            {
                var herbaria = await _http.GetFromJsonAsync<List<HerbariumDetailsResponse>>(
                    "herbaria/me");

                if (herbaria != null)
                    await LocalHerbariaRepository.SaveHerbariaAsync(herbaria);

                foreach (var h in herbaria ?? new List<HerbariumDetailsResponse>())
                {
                    var plants = await _http.GetFromJsonAsync<List<PlantResponse>>(
                        $"herbaria/{h.id}/plants");

                    if (plants != null)
                        await LocalPlantsRepository.SavePlantsAsync(plants);
                }

                LocalDatabaseService.Instance.UpdateLastSyncTime(DateTime.UtcNow);
            }
            catch
            {

            }
        }
    }
}
