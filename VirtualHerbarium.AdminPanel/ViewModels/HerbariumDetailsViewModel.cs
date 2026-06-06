using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;
using VirtualHerbarium.AdminPanel.Views;
using CommunityToolkit.Mvvm.Input;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class HerbariumDetailsViewModel
    {
        private readonly PlantsService _plantsService = PlantsService.Instance;

        public HerbariumDetailsResponse Details { get; }
        public ObservableCollection<PlantResponse> Plants { get; }

        public ICommand ShowPlantDetailsCommand { get; }

        public HerbariumDetailsViewModel(HerbariumDetailsResponse details,
                                         List<PlantResponse> plants)
        {
            Details = details;
            Plants = new ObservableCollection<PlantResponse>(plants);

            ShowPlantDetailsCommand = new RelayCommand<PlantResponse>(async p => await ShowPlantDetails(p));
        }

        public string NameDisplay => $"Name: {Details.name}";
        public string DescriptionDisplay => $"Description: {Details.description}";
        public string CreatedDisplay => $"Created: {Details.createdAt}";
        public string UpdatedDisplay => $"Updated: {Details.updatedAt}";
        public string PublicDisplay => Details.@public ? "Public" : "Private";
        public string PlantCountDisplay => $"Plants: {Details.plantCount}";

        private async Task ShowPlantDetails(PlantResponse plant)
        {
            var result = await _plantsService.GetPlantDetailsAsync(plant.herbariumId, plant.id);

            if (!result.Success || result.Data == null)
            {
                MessageBox.Show(AppResources.Error_Server,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            var window = new PlantDetailsView
            {
                DataContext = new PlantDetailsViewModel(result.Data)
            };

            window.Show();
        }
    }
}
