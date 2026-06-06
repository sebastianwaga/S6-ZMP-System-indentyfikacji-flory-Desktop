using System;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services.Offline;
using System.Windows.Media.Imaging;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class PlantsService
    {
        public static PlantsService Instance { get; } = new PlantsService();

        private PlantsService() { }

        public async Task<PlantPhotoResponse?> GetPhotoMetadataAsync(string herbariumId, string plantId, string photoId)
        {
            try
            {
                if (!InternetService.Instance.IsOnline)
                {
                    var photos = await LocalPhotosRepository.GetPhotosAsync(plantId);
                    return photos.FirstOrDefault(p => p.id == photoId);
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync($"herbaria/{herbariumId}/plants/{plantId}/photos/{photoId}")
                );

                if (response == null || !response.IsSuccessStatusCode)
                    return null;

                var data = await response.Content.ReadFromJsonAsync<PlantPhotoResponse>();

                if (data != null)
                    await LocalPhotosRepository.AddPhotoAsync(data);

                return data;
            }
            catch
            {
                return null;
            }
        }

        public async Task<BitmapImage?> LoadPhotoAsync(string photoId, string url)
        {
            return await PhotoCacheService.LoadPhotoAsync(photoId, url);
        }

        public async Task<ApiResult<List<PlantResponse>>> GetPlantsAsync()
        {
            try
            {
                if (!InternetService.Instance.IsOnline)
                {
                    var offlinePlants = await LocalPlantsRepository.GetPlantsAsync();

                    return new ApiResult<List<PlantResponse>>
                    {
                        Success = true,
                        Data = offlinePlants
                    };
                }
                var herbariaResponse = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync("stats/herbaria")
                );

                if (herbariaResponse == null)
                {
                    return new ApiResult<List<PlantResponse>>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

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
                    var plantsResponse = await AuthService.Instance.SendAuthorizedAsync(
                        http => http.GetAsync($"herbaria/{herbarium.id}/plants")
                    );

                    if (plantsResponse == null || !plantsResponse.IsSuccessStatusCode)
                        continue;

                    var plants = await plantsResponse.Content.ReadFromJsonAsync<List<PlantResponse>>();

                    if (plants != null)
                    {
                        foreach (var p in plants)
                            p.herbariumId = herbarium.id;

                        allPlants.AddRange(plants);
                    }
                }

                await LocalPlantsRepository.SavePlantsAsync(allPlants);

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
                    Error = ex.Message
                };
            }
        }
        public async Task<ApiResult<PlantDetailsResponse>> GetPlantDetailsAsync(string herbariumId, string plantId)
        {
            try
            {
                if (!InternetService.Instance.IsOnline)
                {
                    var photos = await LocalPhotosRepository.GetPhotosAsync(plantId);

                    var offlineDetails = new PlantDetailsResponse
                    {
                        id = plantId,
                        herbariumId = herbariumId,
                        name = "(offline)",
                        createdAt = DateTime.MinValue,
                        updatedAt = DateTime.MinValue,
                        photos = photos
                    };

                    return new ApiResult<PlantDetailsResponse>
                    {
                        Success = true,
                        Data = offlineDetails
                    };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync($"herbaria/{herbariumId}/plants/{plantId}")
                );

                if (response == null)
                {
                    return new ApiResult<PlantDetailsResponse>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResult<PlantDetailsResponse>
                    {
                        Success = false,
                        StatusCode = (int)response.StatusCode
                    };
                }

                var data = await response.Content.ReadFromJsonAsync<PlantDetailsResponse>();

                if (data?.photos != null)
                    await LocalPhotosRepository.SavePhotosAsync(data.photos);

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
                    Error = ex.Message
                };
            }
        }
        public async Task<ApiResult<bool>> DeletePlantAsync(string herbariumId, string plantId)
        {
            try
            {
                if (!InternetService.Instance.IsOnline)
                {
                    await LocalPlantsRepository.DeletePlantAsync(plantId);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "delete",
                        entityType: "plant",
                        entityId: plantId,
                        payload: "{}",
                        baseUpdatedAt: null
                    );

                    return new ApiResult<bool> { Success = true, Data = true };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.DeleteAsync($"herbaria/{herbariumId}/plants/{plantId}")
                );

                if (response == null)
                {
                    return new ApiResult<bool>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    await LocalPlantsRepository.DeletePlantAsync(plantId);
                    return new ApiResult<bool> { Success = true, Data = true };
                }

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
                    Error = ex.Message
                };
            }
        }
        public async Task<ApiResult<bool>> UpdatePlantNameAsync(string herbariumId, string plantId, string newName)
        {
            try
            {
                var body = new { name = newName };

                if (!InternetService.Instance.IsOnline)
                {
                    await LocalPlantsRepository.UpdatePlantAsync(plantId, newName);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "update",
                        entityType: "plant",
                        entityId: plantId,
                        payload: System.Text.Json.JsonSerializer.Serialize(body),
                        baseUpdatedAt: null
                    );

                    return new ApiResult<bool> { Success = true, Data = true };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PatchAsJsonAsync($"herbaria/{herbariumId}/plants/{plantId}", body)
                );

                if (response == null)
                {
                    return new ApiResult<bool>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    await LocalPlantsRepository.UpdatePlantAsync(plantId, newName);
                    return new ApiResult<bool> { Success = true, Data = true };
                }

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
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResult<PlantDetailsResponse>> AddPlantAsync(
            string herbariumId,
            string name,
            string base64Photo)
        {
            try
            {
                var body = new
                {
                    name,
                    photo = base64Photo
                };

                if (!InternetService.Instance.IsOnline)
                {
                    var newPlant = new PlantResponse
                    {
                        id = Guid.NewGuid().ToString(),
                        herbariumId = herbariumId,
                        name = name,
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow
                    };

                    await LocalPlantsRepository.AddPlantAsync(newPlant);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "create",
                        entityType: "plant",
                        entityId: newPlant.id,
                        payload: System.Text.Json.JsonSerializer.Serialize(body),
                        baseUpdatedAt: null
                    );

                    return new ApiResult<PlantDetailsResponse>
                    {
                        Success = true,
                        Data = new PlantDetailsResponse
                        {
                            id = newPlant.id,
                            name = newPlant.name,
                            herbariumId = newPlant.herbariumId,
                            photos = new List<PlantPhotoResponse>()
                        }
                    };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PostAsJsonAsync($"herbaria/{herbariumId}/plants/add", body)
                );

                if (response == null)
                {
                    return new ApiResult<PlantDetailsResponse>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<PlantDetailsResponse>();

                    if (data != null)
                    {
                        await LocalPlantsRepository.AddPlantAsync(new PlantResponse
                        {
                            id = data.id,
                            herbariumId = herbariumId,
                            name = data.name,
                            createdAt = data.createdAt,
                            updatedAt = data.updatedAt
                        });
                    }

                    return new ApiResult<PlantDetailsResponse> { Success = true, Data = data };
                }

                return new ApiResult<PlantDetailsResponse>
                {
                    Success = false,
                    Error = await response.Content.ReadAsStringAsync(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<PlantDetailsResponse>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }
        public async Task<ApiResult<bool>> MovePhotoAsync(
            string herbariumId,
            string plantId,
            string photoId,
            string targetPlantId)
        {
            try
            {
                var body = new { targetPlantId };

                if (!InternetService.Instance.IsOnline)
                {
                    await LocalPhotosRepository.MovePhotoAsync(photoId, targetPlantId);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "move",
                        entityType: "photo",
                        entityId: photoId,
                        payload: System.Text.Json.JsonSerializer.Serialize(body),
                        baseUpdatedAt: null
                    );

                    return new ApiResult<bool> { Success = true, Data = true };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PostAsJsonAsync(
                        $"herbaria/{herbariumId}/plants/{plantId}/photos/{photoId}/move",
                        body)
                );

                if (response == null)
                {
                    return new ApiResult<bool>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    await LocalPhotosRepository.MovePhotoAsync(photoId, targetPlantId);
                    return new ApiResult<bool> { Success = true, Data = true };
                }

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
                    Error = ex.Message
                };
            }
        }
        public async Task<ApiResult<bool>> UpdatePhotoDescriptionAsync(
            string herbariumId,
            string plantId,
            string photoId,
            string description)
        {
            try
            {
                var body = new { description };

                if (!InternetService.Instance.IsOnline)
                {
                    await LocalPhotosRepository.UpdatePhotoDescriptionAsync(photoId, description);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "update",
                        entityType: "photo",
                        entityId: photoId,
                        payload: System.Text.Json.JsonSerializer.Serialize(body),
                        baseUpdatedAt: null
                    );

                    return new ApiResult<bool> { Success = true, Data = true };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PatchAsJsonAsync(
                        $"herbaria/{herbariumId}/plants/{plantId}/photos/{photoId}",
                        body)
                );

                if (response == null)
                {
                    return new ApiResult<bool>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    await LocalPhotosRepository.UpdatePhotoDescriptionAsync(photoId, description);
                    return new ApiResult<bool> { Success = true, Data = true };
                }

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
                    Error = ex.Message
                };
            }
        }
        public async Task<ApiResult<bool>> DeletePhotoAsync(
            string herbariumId,
            string plantId,
            string photoId)
        {
            try
            {
                if (!InternetService.Instance.IsOnline)
                {
                    await LocalPhotosRepository.DeletePhotoAsync(photoId);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "delete",
                        entityType: "photo",
                        entityId: photoId,
                        payload: "{}",
                        baseUpdatedAt: null
                    );

                    return new ApiResult<bool> { Success = true, Data = true };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.DeleteAsync(
                        $"herbaria/{herbariumId}/plants/{plantId}/photos/{photoId}")
                );

                if (response == null)
                {
                    return new ApiResult<bool>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    await LocalPhotosRepository.DeletePhotoAsync(photoId);
                    return new ApiResult<bool> { Success = true, Data = true };
                }

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
                    Error = ex.Message
                };
            }
        }
    }
}