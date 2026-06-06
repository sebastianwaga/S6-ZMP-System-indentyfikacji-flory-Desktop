namespace VirtualHerbarium.AdminPanel.Models
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; }

        public static ApiResult<T> Unauthorized()
        {
            return new ApiResult<T>
            {
                Success = false,
                StatusCode = 401,
                Error = AppResources.Error_SessionExpired
            };
        }

        public static ApiResult<T> ServerError()
        {
            return new ApiResult<T>
            {
                Success = false,
                StatusCode = 500,
                Error = AppResources.Error_Server
            };
        }

        public static ApiResult<T> SuccessTrue()
        {
            return new ApiResult<T>
            {
                Success = true,
                Data = (T)(object)true
            };
        }

    }
}
