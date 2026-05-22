namespace VirtualHerbarium.AdminPanel.Models
{
    public class UserDetailsResponse
    {
        public string id { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public bool active { get; set; }
        public bool verified { get; set; }
        public bool admin { get; set; }

        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        public int herbariumCount { get; set; }
        public int plantCount { get; set; }
        public int photoCount { get; set; }
        public int friendCount { get; set; }

        public List<UserHerbariumInfo> herbaria { get; set; }
    }

    public class UserHerbariumInfo
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
