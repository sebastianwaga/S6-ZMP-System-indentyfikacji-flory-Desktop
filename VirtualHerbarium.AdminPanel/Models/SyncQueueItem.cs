namespace VirtualHerbarium.AdminPanel.Models
{
    public class SyncQueueItem
    {
        public int Id { get; set; }
        public string ActionType { get; set; }   
        public string EntityType { get; set; }  
        public string EntityId { get; set; }
        public string Payload { get; set; }
        public string BaseUpdatedAt { get; set; }
        public int RetryCount { get; set; }
        public bool Sent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
