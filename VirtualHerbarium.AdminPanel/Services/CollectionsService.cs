using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class CollectionsService
    {
        private readonly bool _useMock = true;

        public async Task<ApiResult<List<CollectionResponse>>> GetCollectionsAsync()
        {
            if (_useMock)
            {
                await Task.Delay(200);

                return new ApiResult<List<CollectionResponse>>
                {
                    Success = true,
                    Data = new List<CollectionResponse>
                    {
                        new CollectionResponse { id="1", name="Zioła lecznicze", owner="user1", plantCount=12, shared=true },
                        new CollectionResponse { id="2", name="Drzewa iglaste", owner="user2", plantCount=8, shared=false },
                        new CollectionResponse { id="3", name="Rośliny wodne", owner="admin", plantCount=5, shared=true }
                    }
                };
            }

            try
            {
                throw new NotImplementedException("API jeszcze nie działa.");
            }
            catch (Exception ex)
            {
                return new ApiResult<List<CollectionResponse>>
                {
                    Success = false,
                    Error = ex.Message,
                    StatusCode = 0
                };
            }
        }

        public async Task<ApiResult<bool>> DeleteCollectionAsync(string id)
        {
            if (_useMock)
            {
                await Task.Delay(100);
                return new ApiResult<bool> { Success = true, Data = true };
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
