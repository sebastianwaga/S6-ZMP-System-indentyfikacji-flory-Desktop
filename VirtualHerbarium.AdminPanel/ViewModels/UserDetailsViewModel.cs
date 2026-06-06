using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class UserDetailsViewModel
    {
        public UserDetailsResponse Data { get; }

        public UserDetailsViewModel(UserDetailsResponse data)
        {
            Data = data;
        }

        public string UsernameDisplay => $"User: {Data.username}";
        public string EmailDisplay => $"Email: {Data.email}";
        public string CreatedDisplay => $"Created: {Data.createdAt}";
        public string UpdatedDisplay => $"Updated: {Data.updatedAt}";

        public string StatsDisplay =>
            $"Herbaria: {Data.herbariumCount}, Plants: {Data.plantCount}, Photos: {Data.photoCount}, Friends: {Data.friendCount}";

        public object Herbaria => Data.herbaria;
    }
}
