using CommunityToolkit.Mvvm.Input;
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
        private readonly PlantsService _service = PlantsService.Instance;

        public ObservableCollection<PlantResponse> Plants { get; set; }

        public ICommand DeletePlantCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ShowPlantDetailsCommand { get; }
        public ICommand EditPlantNameCommand { get; }
        public ICommand AddPlantCommand { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public PlantsViewModel()
        {
            Plants = new ObservableCollection<PlantResponse>();

            DeletePlantCommand = new RelayCommand<PlantResponse>(
            async p => await DeletePlant(p),
            p => p != null
            );

            RefreshCommand = new RelayCommand<object>(async _ => await LoadPlants());
            ShowPlantDetailsCommand = new RelayCommand<PlantResponse>(
            async p => await ShowPlantDetails(p),
            p => p != null
            );


            EditPlantNameCommand = new RelayCommand<PlantResponse>(
            async p => await EditPlantName(p),
            p => p != null
            );

            AddPlantCommand = new RelayCommand(async () => await AddPlant());

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
            if (MessageBox.Show($"Delete plant \"{plant.name}\"?",
            "Confirmation",
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
                MessageBox.Show(result.Error ?? "Failed to load plant details.",
                 "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            var window = new PlantDetailsView
            {
                DataContext = new PlantDetailsViewModel(result.Data)
            };

            window.Show();
        }

        private async Task EditPlantName(PlantResponse plant)
        {
            var newName = Microsoft.VisualBasic.Interaction.InputBox(
            "Enter new plant name:",
            "Edit name",
            plant.name);


            if (string.IsNullOrWhiteSpace(newName))
                return;

            IsBusy = true;

            var result = await _service.UpdatePlantNameAsync(plant.herbariumId, plant.id, newName);

            if (!result.Success)
            {
                MessageBox.Show("Failed to change name.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                IsBusy = false;
                return;
            }

            plant.name = newName;
            OnPropertyChanged(nameof(Plants));

            IsBusy = false;
        }

        private async Task AddPlant()
        {
            var dialog = new AddPlantDialog();
            if (dialog.ShowDialog() != true)
                return;

            var name = dialog.PlantName;
            var photoBase64 = dialog.PhotoBase64;
            var herbariumId = dialog.HerbariumId;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(photoBase64))
            {
                MessageBox.Show("Fill in all fields.");
                return;
            }

            IsBusy = true;

            var result = await _service.AddPlantAsync(herbariumId, name, photoBase64);

            if (!result.Success || result.Data == null)
            {
                MessageBox.Show("Failed to add plant.");
                IsBusy = false;
                return;
            }

            Plants.Add(new PlantResponse
            {
                id = result.Data.id,
                name = result.Data.name,
                herbariumId = result.Data.herbariumId
            });

            IsBusy = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
