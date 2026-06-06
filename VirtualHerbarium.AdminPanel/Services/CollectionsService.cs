using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services.Offline;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class CollectionsService
    {
        public CollectionsService() { }

        private ApiResult<T> HandleError<T>(HttpResponseMessage response)
        {
            int code = (int)response.StatusCode;

            return new ApiResult<T>
            {
                Success = false,
                StatusCode = code,
                Error = code switch
                {
                    401 => AppResources.Error_SessionExpired,
                    403 => AppResources.Error_NoPermission,
                    404 => AppResources.Error_UserNotFound,
                    _ => AppResources.Error_Server
                }
            };
        }

        public async Task<ApiResult<List<HerbariumStatsResponse>>> GetCollectionsAsync()
        {
            try
            {
                if (!InternetService.Instance.IsOnline)
                {
                    var offline = await LocalHerbariaRepository.GetHerbariaAsync();

                    var stats = offline.ConvertAll(h => new HerbariumStatsResponse
                    {
                        id = h.id,
                        name = h.name,
                        ownerId = h.userId,
                        ownerUsername = null
                    });

                    return new ApiResult<List<HerbariumStatsResponse>>
                    {
                        Success = true,
                        Data = stats
                    };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync("stats/herbaria")
                );

                if (response == null)
                {
                    return new ApiResult<List<HerbariumStatsResponse>>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (!response.IsSuccessStatusCode)
                {
                    return HandleError<List<HerbariumStatsResponse>>(response);
                }

                var data = await response.Content.ReadFromJsonAsync<List<HerbariumStatsResponse>>();

                var fullList = new List<HerbariumDetailsResponse>();

                foreach (var h in data)
                {
                    var details = await GetHerbariumDetailsAsync(h.id);
                    if (details.Success && details.Data != null)
                        fullList.Add(details.Data);
                }

                await LocalHerbariaRepository.SaveHerbariaAsync(fullList);

                return new ApiResult<List<HerbariumStatsResponse>>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<HerbariumStatsResponse>>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> DeleteCollectionAsync(string userId, string herbariumId)
        {
            try
            {
                if (!InternetService.Instance.IsOnline)
                {
                    await LocalHerbariaRepository.DeleteHerbariumAsync(herbariumId);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "delete",
                        entityType: "herbarium",
                        entityId: herbariumId,
                        payload: "{}",
                        baseUpdatedAt: null
                    );

                    return new ApiResult<bool> { Success = true, Data = true };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.DeleteAsync($"herbaria/{herbariumId}")
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
                    await LocalHerbariaRepository.DeleteHerbariumAsync(herbariumId);
                    return new ApiResult<bool> { Success = true, Data = true };
                }

                return HandleError<bool>(response);
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
        public async Task<ApiResult<HerbariumDetailsResponse>> GetHerbariumDetailsAsync(string id)
        {
            try
            {
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync($"herbaria/{id}")
                );

                if (response == null)
                {
                    return new ApiResult<HerbariumDetailsResponse>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<HerbariumDetailsResponse>();
                    return new ApiResult<HerbariumDetailsResponse> { Success = true, Data = data };
                }

                return HandleError<HerbariumDetailsResponse>(response);
            }
            catch
            {
                return new ApiResult<HerbariumDetailsResponse>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }
        public async Task<ApiResult<List<PlantResponse>>> GetHerbariumPlantsAsync(string id)
        {
            try
            {
                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.GetAsync($"herbaria/{id}/plants")
                );

                if (response == null)
                {
                    return new ApiResult<List<PlantResponse>>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<PlantResponse>>();
                    return new ApiResult<List<PlantResponse>> { Success = true, Data = data };
                }

                return HandleError<List<PlantResponse>>(response);
            }
            catch
            {
                return new ApiResult<List<PlantResponse>>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }
        public async Task<ApiResult<HerbariumDetailsResponse>> CreateHerbariumAsync(
        string name, string description, bool isPublic)
        {
            try
            {
                var body = new
                {
                    name,
                    description,
                    @public = isPublic
                };

                if (!InternetService.Instance.IsOnline)
                {
                    var newHerbarium = new HerbariumDetailsResponse
                    {
                        id = Guid.NewGuid().ToString(),
                        userId = "",
                        name = name,
                        description = description,
                        @public = isPublic,
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow
                    };

                    await LocalHerbariaRepository.AddHerbariumAsync(newHerbarium);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "create",
                        entityType: "herbarium",
                        entityId: newHerbarium.id,
                        payload: System.Text.Json.JsonSerializer.Serialize(body),
                        baseUpdatedAt: null
                    );

                    return new ApiResult<HerbariumDetailsResponse>
                    {
                        Success = true,
                        Data = newHerbarium
                    };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PostAsJsonAsync("herbaria", body)
                );

                if (response == null)
                {
                    return new ApiResult<HerbariumDetailsResponse>
                    {
                        Success = false,
                        Error = "UNAUTHORIZED",
                        StatusCode = 401
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<HerbariumDetailsResponse>();

                    if (data != null)
                        await LocalHerbariaRepository.AddHerbariumAsync(data);

                    return new ApiResult<HerbariumDetailsResponse>
                    {
                        Success = true,
                        Data = data
                    };
                }

                return HandleError<HerbariumDetailsResponse>(response);
            }
            catch
            {
                return new ApiResult<HerbariumDetailsResponse>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }
        public async Task<ApiResult<bool>> UpdateHerbariumAsync(
            string id, string name, string description, bool isPublic)
        {
            try
            {
                var body = new
                {
                    name,
                    description,
                    @public = isPublic
                };

                if (!InternetService.Instance.IsOnline)
                {
                    await LocalHerbariaRepository.UpdateHerbariumAsync(id, name, description, isPublic);

                    LocalDatabaseService.Instance.AddToSyncQueue(
                        actionType: "update",
                        entityType: "herbarium",
                        entityId: id,
                        payload: System.Text.Json.JsonSerializer.Serialize(body),
                        baseUpdatedAt: null
                    );

                    return new ApiResult<bool> { Success = true, Data = true };
                }

                var response = await AuthService.Instance.SendAuthorizedAsync(
                    http => http.PatchAsJsonAsync($"herbaria/{id}", body)
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
                    await LocalHerbariaRepository.UpdateHerbariumAsync(id, name, description, isPublic);
                    return new ApiResult<bool> { Success = true, Data = true };
                }

                return HandleError<bool>(response);
            }
            catch
            {
                return new ApiResult<bool>
                {
                    Success = false,
                    Error = AppResources.Error_Server
                };
            }
        }
    }
}


