using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;
using VirtualHerbarium.AdminPanel.Views;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class CollectionsViewModel : INotifyPropertyChanged
    {
        private readonly CollectionsService _service;

        public ObservableCollection<HerbariumStatsResponse> Collections { get; set; }

        public ICommand DeleteCommand { get; }
        public ICommand ShowDetailsCommand { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public CollectionsViewModel()
        {
            _service = new CollectionsService();
            Collections = new ObservableCollection<HerbariumStatsResponse>();

            DeleteCommand = new RelayCommand<HerbariumStatsResponse>(async h => await DeleteCollection(h));
            ShowDetailsCommand = new RelayCommand<HerbariumStatsResponse>(async h => await ShowDetails(h));

            LoadCollections();
        }

        private async void LoadCollections()
        {
            IsBusy = true;

            var result = await _service.GetCollectionsAsync();

            if (!result.Success || result.Data == null)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }

            Collections = new ObservableCollection<HerbariumStatsResponse>(result.Data);
            OnPropertyChanged(nameof(Collections));

            IsBusy = false;
        }

        private async Task DeleteCollection(HerbariumStatsResponse herbarium)
        {
            if (MessageBox.Show($"Na pewno usunąć zielnik \"{herbarium.name}\"?",
                    "Potwierdzenie",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            IsBusy = true;

            var result = await _service.DeleteCollectionAsync(herbarium.ownerId, herbarium.id);

            if (!result.Success)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }

            Collections.Remove(herbarium);
            MessageBox.Show("Zielnik został usunięty.",
                AppResources.Success_Title, MessageBoxButton.OK, MessageBoxImage.Information);

            IsBusy = false;
        }

        private async Task ShowDetails(HerbariumStatsResponse herbarium)
        {
            IsBusy = true;

            var details = await _service.GetHerbariumDetailsAsync(herbarium.id);
            var plants = await _service.GetHerbariumPlantsAsync(herbarium.id);

            if (!details.Success || details.Data == null)
            {
                ShowError(details.StatusCode, details.Error);
                IsBusy = false;
                return;
            }

            if (!plants.Success || plants.Data == null)
            {
                ShowError(plants.StatusCode, plants.Error);
                IsBusy = false;
                return;
            }

            IsBusy = false;

            var window = new HerbariumDetailsView
            {
                DataContext = new HerbariumDetailsViewModel(details.Data, plants.Data)
            };

            window.Show();
        }

        private void ShowError(int code, string? error)
        {
            string msg = code switch
            {
                401 => AppResources.Error_SessionExpired,
                403 => AppResources.Error_NoPermission,
                404 => AppResources.Error_UserNotFound,
                _ => $"{AppResources.Error_Generic}:"
            };

            MessageBox.Show(msg, AppResources.Error_Title,
                MessageBoxButton.OK, MessageBoxImage.Error);

            if (code == 401)
            {
                AuthService.Instance.Logout();
                Application.Current.Shutdown();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}

