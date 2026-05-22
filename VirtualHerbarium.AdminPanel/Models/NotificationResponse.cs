namespace VirtualHerbarium.AdminPanel.Models
{
    public class NotificationResponse
    {
        public string id { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public bool read { get; set; }
        public string createdAt { get; set; }
    }
}
