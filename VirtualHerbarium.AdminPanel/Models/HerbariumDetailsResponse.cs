namespace VirtualHerbarium.AdminPanel.Models
{
    public class HerbariumDetailsResponse
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int plantCount { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public bool @public { get; set; }
    }
}
