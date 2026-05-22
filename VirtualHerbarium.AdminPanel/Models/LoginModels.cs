namespace VirtualHerbarium.AdminPanel.Models
{
    public class LoginRequest
    {
        public string login { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string? message { get; set; }
        public string? warning { get; set; }
        public string? id { get; set; }
        public string? username { get; set; }
        public string? email { get; set; }
        public bool verified { get; set; }
        public bool admin { get; set; }
        public string? token { get; set; }
    }
}
