using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VirtualHerbarium.AdminPanel.Helpers;
using VirtualHerbarium.AdminPanel.Services;
using VirtualHerbarium.AdminPanel.Views;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public static MainViewModel Instance { get; } = new MainViewModel();

        [ObservableProperty]
        private object? currentView;

        private string? selectedMenu;
        public string? SelectedMenu
        {
            get => selectedMenu;
            set
            {
                SetProperty(ref selectedMenu, value);
                RefreshCurrentView();
            }
        }

        private bool isOnline = true;
        public bool IsOnline
        {
            get => isOnline;
            private set
            {
                if (SetProperty(ref isOnline, value))
                {
                    OnPropertyChanged(nameof(IsOffline));
                }
            }
        }

        public bool IsOffline => !IsOnline;

        public ICommand ChangeLanguageCommand { get; }
        public ICommand LogoutCommand { get; }

        private MainViewModel()
        {
            ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguage);
            LogoutCommand = new RelayCommand(Logout);

            CurrentView = new PlaceholderViewModel(AppResources.Placeholder_SelectOption);

            InternetService.Instance.InternetStatusChanged += status =>
            {
                IsOnline = status;
            };

            _ = InternetService.Instance.ForceCheck();
        }

        private void ChangeLanguage(string lang)
        {
            LanguageManager.SetLanguage(lang);

            var temp = SelectedMenu;
            SelectedMenu = null;
            SelectedMenu = temp;
        }

        private void RefreshCurrentView()
        {
            switch (SelectedMenu)
            {
                case "Users":
                    CurrentView = new UsersView { DataContext = new UsersViewModel() };
                    break;

                case "Plants":
                    CurrentView = new PlantsView { DataContext = new PlantsViewModel() };
                    break;

                case "Collections":
                    CurrentView = new CollectionsView { DataContext = new CollectionsViewModel() };
                    break;

                case "Notifications":
                    CurrentView = new NotificationsView { DataContext = new NotificationsViewModel() };
                    break;

                case "Stats":
                    CurrentView = new StatsView { DataContext = new StatsViewModel() };
                    break;

                default:
                    CurrentView = new PlaceholderViewModel(AppResources.Placeholder_SelectOption);
                    break;
            }
        }

        private void Logout()
        {
            AuthService.Instance.Logout();

            var main = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();

            main?.Close();
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
