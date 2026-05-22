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

        public string UsernameDisplay => $"Użytkownik: {Data.username}";
        public string EmailDisplay => $"Email: {Data.email}";
        public string CreatedDisplay => $"Utworzono: {Data.createdAt}";
        public string UpdatedDisplay => $"Zaktualizowano: {Data.updatedAt}";

        public string StatsDisplay =>
            $"Zielników: {Data.herbariumCount}, Roślin: {Data.plantCount}, Zdjęć: {Data.photoCount}, Znajomych: {Data.friendCount}";

        public object Herbaria => Data.herbaria;
    }
}
