using CommunityToolkit.Mvvm.ComponentModel;
using VirtualHerbarium.AdminPanel.Views;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? currentView;

        [ObservableProperty]
        private string? selectedMenu;

        public MainViewModel()
        {
            CurrentView = new PlaceholderViewModel("Wybierz opcję z menu");
        }

        partial void OnSelectedMenuChanged(string? value)
        {
            if (value == null)
                return;

            switch (value)
            {
                case "Users":
                    CurrentView = new UsersView(); 
                    break;

                case "Plants":
                    CurrentView = new PlantsView();
                    break;

                case "Collections":
                    CurrentView = new PlaceholderViewModel("Widok zbiorów");
                    break;

                case "Notifications":
                    CurrentView = new PlaceholderViewModel("Widok powiadomień");
                    break;

                case "Stats":
                    CurrentView = new PlaceholderViewModel("Widok statystyk");
                    break;
            }
        }
    }

    public class PlaceholderViewModel
    {
        public string Text { get; }

        public PlaceholderViewModel(string text)
        {
            Text = text;
        }
    }
}
