namespace VirtualHerbarium.AdminPanel.Models
{
    public class StatsOverviewResponse
    {
        public UsersStats users { get; set; }
        public HerbariaStats herbaria { get; set; }
        public PlantsStats plants { get; set; }
        public FriendshipsStats friendships { get; set; }
    }

    public class UsersStats
    {
        public int totalUsers { get; set; }
        public int activeUsers { get; set; }
        public int inactiveUsers { get; set; }
        public int verifiedUsers { get; set; }
        public int unverifiedUsers { get; set; }
        public int admins { get; set; }
    }

    public class HerbariaStats
    {
        public int totalHerbaria { get; set; }
        public int publicHerbaria { get; set; }
        public int privateHerbaria { get; set; }
    }

    public class PlantsStats
    {
        public int totalPlants { get; set; }
        public int recognizedPlants { get; set; }
        public int unrecognizedPlants { get; set; }
        public int totalPhotos { get; set; }
    }

    public class FriendshipsStats
    {
        public int totalFriendships { get; set; }
        public int pendingRequests { get; set; }
    }
}
