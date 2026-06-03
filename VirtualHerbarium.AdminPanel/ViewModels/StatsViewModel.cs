using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public partial class StatsViewModel : ObservableObject
    {
        [ObservableProperty] private int totalUsers;
        [ObservableProperty] private int activeUsers;
        [ObservableProperty] private int inactiveUsers;
        [ObservableProperty] private int verifiedUsers;
        [ObservableProperty] private int unverifiedUsers;
        [ObservableProperty] private int admins;

        [ObservableProperty] private int totalHerbaria;
        [ObservableProperty] private int publicHerbaria;
        [ObservableProperty] private int privateHerbaria;

        [ObservableProperty] private int totalPlants;
        [ObservableProperty] private int recognizedPlants;
        [ObservableProperty] private int unrecognizedPlants;
        [ObservableProperty] private int totalPhotos;

        [ObservableProperty] private int totalFriendships;
        [ObservableProperty] private int pendingRequests;

        public StatsViewModel()
        {
            LoadStats();
        }

        private async void LoadStats()
        {
            var result = await StatsService.Instance.GetOverviewAsync();

            if (!result.Success || result.Data == null)
                return;

            var u = result.Data.users;
            var h = result.Data.herbaria;
            var p = result.Data.plants;
            var f = result.Data.friendships;

            TotalUsers = u.totalUsers;
            ActiveUsers = u.activeUsers;
            InactiveUsers = u.inactiveUsers;
            VerifiedUsers = u.verifiedUsers;
            UnverifiedUsers = u.unverifiedUsers;
            Admins = u.admins;

            TotalHerbaria = h.totalHerbaria;
            PublicHerbaria = h.publicHerbaria;
            PrivateHerbaria = h.privateHerbaria;

            TotalPlants = p.totalPlants;
            RecognizedPlants = p.recognizedPlants;
            UnrecognizedPlants = p.unrecognizedPlants;
            TotalPhotos = p.totalPhotos;

            TotalFriendships = f.totalFriendships;
            PendingRequests = f.pendingRequests;
        }
    }
}
