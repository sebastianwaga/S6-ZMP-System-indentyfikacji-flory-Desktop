using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class PlantDetailsViewModel : INotifyPropertyChanged
    {
        private readonly PlantsService _service;

        public PlantDetailsResponse Plant { get; }
        public ObservableCollection<PlantPhotoResponse> Photos { get; }

        public PlantDetailsViewModel(PlantDetailsResponse plant, PlantsService service)
        {
            _service = service;
            Plant = plant;
            Photos = new ObservableCollection<PlantPhotoResponse>(plant.photos);

            _ = LoadPhotos();
        }

        private async Task LoadPhotos()
        {
            foreach (var p in Photos)
            {
                var meta = await _service.GetPhotoMetadataAsync(Plant.herbariumId, Plant.id, p.id);

                if (meta == null)
                    continue;

                p.Image = await _service.LoadPhotoAsync(meta.url);
            }

            OnPropertyChanged(nameof(Photos));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
