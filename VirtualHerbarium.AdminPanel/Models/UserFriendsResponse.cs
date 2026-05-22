namespace VirtualHerbarium.AdminPanel.Models
{
    public class UserFriendsResponse
    {
        public List<FriendInfo> accepted { get; set; }
        public List<FriendInfo> incoming { get; set; }
        public List<FriendInfo> sent { get; set; }
    }

    public class FriendInfo
    {
        public string friendshipId { get; set; }
        public string userId { get; set; }
        public string username { get; set; }
        public string status { get; set; }
        public string direction { get; set; }
        public DateTime createdAt { get; set; }
    }
}
