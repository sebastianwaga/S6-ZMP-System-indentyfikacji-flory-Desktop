using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public partial class PlantDetailsViewModel : ObservableObject
    {
        public PlantDetailsResponse Plant { get; }
        public ObservableCollection<PhotoViewModel> Photos { get; }

        public PlantDetailsViewModel(PlantDetailsResponse plant)
        {
            Plant = plant;

            Photos = new ObservableCollection<PhotoViewModel>(
                plant.photos.Select(p => new PhotoViewModel(p))
            );

            _ = LoadPhotos();
        }

        private async Task LoadPhotos()
        {
            foreach (var p in Photos)
            {
                var meta = await PlantsService.Instance.GetPhotoMetadataAsync(
                    Plant.herbariumId,
                    Plant.id,
                    p.Metadata.id
                );

                if (meta != null)
                    p.Metadata.url = meta.url;

                await p.LoadAsync();
            }

            OnPropertyChanged(nameof(Photos));
        }
    }
}
