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
    public class PlantsViewModel : INotifyPropertyChanged
    {
        private readonly PlantsService _service = new PlantsService();

        public ObservableCollection<PlantResponse> Plants { get; set; }

        public ICommand DeletePlantCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ShowPlantDetailsCommand { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public PlantsViewModel()
        {
            Plants = new ObservableCollection<PlantResponse>();

            DeletePlantCommand = new RelayCommand<PlantResponse>(async p => await DeletePlant(p));
            RefreshCommand = new RelayCommand<object>(async _ => await LoadPlants());
            ShowPlantDetailsCommand = new RelayCommand<PlantResponse>(async p => await ShowPlantDetails(p));

            LoadPlants();
        }

        private async Task LoadPlants()
        {
            IsBusy = true;

            var result = await _service.GetPlantsAsync();

            if (!result.Success || result.Data == null)
            {
                MessageBox.Show(AppResources.Error_Server,
                AppResources.Error_Title, MessageBoxButton.OK, MessageBoxImage.Error);
                IsBusy = false;
                return;
            }

            Plants = new ObservableCollection<PlantResponse>(result.Data);
            OnPropertyChanged(nameof(Plants));

            IsBusy = false;
        }

        private async Task DeletePlant(PlantResponse plant)
        {
            if (MessageBox.Show($"Usunąć roślinę \"{plant.name}\"?",
                    "Potwierdzenie",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            IsBusy = true;

            var result = await _service.DeletePlantAsync(plant.herbariumId, plant.id);

            if (!result.Success)
            {
                MessageBox.Show(AppResources.Error_Server,
                AppResources.Error_Title, MessageBoxButton.OK, MessageBoxImage.Error);

                IsBusy = false;
                return;
            }

            Plants.Remove(plant);
            IsBusy = false;
        }

        private async Task ShowPlantDetails(PlantResponse plant)
        {
            var result = await _service.GetPlantDetailsAsync(plant.herbariumId, plant.id);

            if (!result.Success || result.Data == null)
            {
                MessageBox.Show(result.Error ?? "Nie udało się pobrać szczegółów rośliny.",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var window = new PlantDetailsView
            {
                DataContext = new PlantDetailsViewModel(result.Data, _service)
            };

            window.Show();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
