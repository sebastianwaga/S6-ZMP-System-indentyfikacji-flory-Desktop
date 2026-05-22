namespace VirtualHerbarium.AdminPanel.Models
{
    public class AdminUserResponse
    {
        public string? id { get; set; }
        public string? email { get; set; }
        public string? username { get; set; }
        public bool active { get; set; }
        public bool verified { get; set; }
        public bool admin { get; set; }
    }
}
