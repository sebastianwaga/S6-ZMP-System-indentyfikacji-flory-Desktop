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
                ShowError(result.Error ?? "Nie udało się pobrać listy roślin.");
                return;
            }

            Plants = new ObservableCollection<PlantResponse>(result.Data);
            OnPropertyChanged(nameof(Plants));
        }

        private async Task DeletePlant(PlantResponse plant)
        {
            var confirm = MessageBox.Show(
                $"Czy na pewno chcesz usunąć roślinę: {plant.name}?",
                "Potwierdzenie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            var result = await _service.DeletePlantAsync(plant.id);

            if (!result.Success)
            {
                ShowError(result.Error ?? "Nie udało się usunąć rośliny.");
                return;
            }

            Plants.Remove(plant);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
