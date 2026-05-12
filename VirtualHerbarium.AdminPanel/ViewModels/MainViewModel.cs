using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VirtualHerbarium.AdminPanel.Helpers;
using VirtualHerbarium.AdminPanel.Views;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? currentView;

        [ObservableProperty]
        private string? selectedMenu;

        public ICommand ChangeLanguageCommand { get; }

        public MainViewModel()
        {
            ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguage);

            CurrentView = new PlaceholderViewModel(AppResources.Placeholder_SelectOption);
        }

        private Task ChangeLanguage(string lang)
        {
            LanguageManager.SetLanguage(lang);

            RefreshCurrentView();

            return Task.CompletedTask;
        }

        private void RefreshCurrentView()
        {
            switch (SelectedMenu)
            {
                case "Users":
                    CurrentView = new UsersView();
                    break;

                case "Plants":
                    CurrentView = new PlantsView();
                    break;

                case "Collections":
                    CurrentView = new CollectionsView();
                    break;

                case "Notifications":
                    CurrentView = new NotificationsView();
                    break;

                case "Stats":
                    CurrentView = new StatsView();
                    break;

                default:
                    CurrentView = new PlaceholderViewModel(AppResources.Placeholder_SelectOption);
                    break;
            }
        }

        partial void OnSelectedMenuChanged(string? value)
        {
            RefreshCurrentView();
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
