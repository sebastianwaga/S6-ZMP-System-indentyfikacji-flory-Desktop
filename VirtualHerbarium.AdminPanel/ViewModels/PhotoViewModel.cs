using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public partial class PhotoViewModel : ObservableObject
    {
        public PlantPhotoResponse Metadata { get; }

        [ObservableProperty]
        private BitmapImage? image;

        public PhotoViewModel(PlantPhotoResponse metadata)
        {
            Metadata = metadata;
        }

        public async Task LoadAsync()
        {
            Image = await PlantsService.Instance.LoadPhotoAsync(Metadata.url);
        }
    }
}
