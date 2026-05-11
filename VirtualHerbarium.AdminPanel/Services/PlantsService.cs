using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class PlantsService
    {
        private readonly bool _useMock = true;
        private readonly HttpClient _http;

        public PlantsService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080")
            };
        }

        public async Task<ApiResult<List<PlantResponse>>> GetPlantsAsync()
        {
            if (_useMock)
            {
                await Task.Delay(200);

                return new ApiResult<List<PlantResponse>>
                {
                    Success = true,
                    Data = new List<PlantResponse>
                    {
                        new PlantResponse { id="1", name="Dąb szypułkowy", species="Quercus robur", owner="user1", verified=true },
                        new PlantResponse { id="2", name="Sosna zwyczajna", species="Pinus sylvestris", owner="user2", verified=false },
                        new PlantResponse { id="3", name="Brzoza brodawkowata", species="Betula pendula", owner="admin", verified=true }
                    }
                };
            }

            try
            {
                var response = await _http.GetAsync("/plants");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<PlantResponse>>();
                    return new ApiResult<List<PlantResponse>> { Success = true, Data = data };
                }

                return new ApiResult<List<PlantResponse>>
                {
                    Success = false,
                    Error = await response.Content.ReadAsStringAsync(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<PlantResponse>>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> DeletePlantAsync(string id)
        {
            if (_useMock)
            {
                await Task.Delay(100);
                return new ApiResult<bool> { Success = true, Data = true };
            }

            try
            {
                var response = await _http.DeleteAsync($"/plants/{id}");

                if (response.IsSuccessStatusCode)
                    return new ApiResult<bool> { Success = true, Data = true };

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
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }
    }
}
