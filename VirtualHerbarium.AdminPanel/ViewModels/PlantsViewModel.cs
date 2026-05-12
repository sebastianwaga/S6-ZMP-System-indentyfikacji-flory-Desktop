using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class PlantsViewModel : INotifyPropertyChanged
    {
        private readonly PlantsService _service = new PlantsService();

        public ObservableCollection<PlantResponse> Plants { get; set; }

        public ICommand DeleteCommand { get; }

        public PlantsViewModel()
        {
            Plants = new ObservableCollection<PlantResponse>();
            DeleteCommand = new RelayCommand<PlantResponse>(async p => await DeletePlant(p));

            LoadPlants();
        }

        private async void LoadPlants()
        {
            var result = await _service.GetPlantsAsync();

            if (!result.Success || result.Data == null)
            {
                ShowError(AppResources.Error_FetchPlants);
                return;
            }

            Plants = new ObservableCollection<PlantResponse>(result.Data);
            OnPropertyChanged(nameof(Plants));
        }

        private async Task DeletePlant(PlantResponse plant)
        {
            var confirm = MessageBox.Show(
                string.Format(AppResources.Confirm_DeletePlant, plant.name),
                AppResources.Confirm_Title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            var result = await _service.DeletePlantAsync(plant.id);

            if (!result.Success)
            {
                ShowError(AppResources.Error_DeletePlant);
                return;
            }

            Plants.Remove(plant);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                AppResources.Error_Title,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
