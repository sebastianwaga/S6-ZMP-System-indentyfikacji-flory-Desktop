namespace VirtualHerbarium.AdminPanel.Models
{
    public class StatsOverviewResponse
    {
        public int totalUsers { get; set; }
        public int totalHerbaria { get; set; }
        public int totalPlants { get; set; }
        public int totalPhotos { get; set; }
        public int totalFriends { get; set; }
        public int totalPendingPhotos { get; set; }
        public int totalWarnings { get; set; }
        public int totalAdmins { get; set; }
    }
}
