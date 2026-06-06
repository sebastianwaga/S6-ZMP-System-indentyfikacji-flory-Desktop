using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;
using VirtualHerbarium.AdminPanel.Views;


namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public partial class PlantDetailsViewModel : ObservableObject
    {
        public PlantDetailsResponse Plant { get; }
        public ObservableCollection<PhotoViewModel> Photos { get; }

        public ICommand EditPhotoDescriptionCommand { get; }
        public ICommand DeletePhotoCommand { get; }
        public ICommand MovePhotoCommand { get; }

        public PlantDetailsViewModel(PlantDetailsResponse plant)
        {
            Plant = plant;

            Photos = new ObservableCollection<PhotoViewModel>(
                plant.photos.Select(p => new PhotoViewModel(p))
            );

            EditPhotoDescriptionCommand = new RelayCommand<PhotoViewModel>(
            async p => await EditDescription(p),
            p => p != null
            );

            DeletePhotoCommand = new RelayCommand<PhotoViewModel>(async p => await DeletePhoto(p)); DeletePhotoCommand = new RelayCommand<PhotoViewModel>(
            async p => await DeletePhoto(p),
            p => p != null
            );

            MovePhotoCommand = new RelayCommand<PhotoViewModel>(
            async p => await MovePhoto(p),
            p => p != null
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


        private async Task EditDescription(PhotoViewModel photo)
        {
            var newDesc = Microsoft.VisualBasic.Interaction.InputBox(
            "New photo description:",
            "Edit description",
            photo.Metadata.description);


            if (string.IsNullOrWhiteSpace(newDesc))
                return;

            var result = await PlantsService.Instance.UpdatePhotoDescriptionAsync(
                Plant.herbariumId,
                Plant.id,
                photo.Metadata.id,
                newDesc);

            if (!result.Success)
            {
                MessageBox.Show("Failed to change description.");
                return;
            }

            photo.Metadata.description = newDesc;
            OnPropertyChanged(nameof(Photos));
        }
        private async Task DeletePhoto(PhotoViewModel photo)
        {
            if (MessageBox.Show("Delete this photo?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;


            var result = await PlantsService.Instance.DeletePhotoAsync(
                Plant.herbariumId,
                Plant.id,
                photo.Metadata.id);

            if (!result.Success)
            {
                MessageBox.Show("Failed to delete photo.");
                return;
            }

            Photos.Remove(photo);
        }
        private async Task MovePhoto(PhotoViewModel photo)
        {
            var dialog = new MovePhotoDialog(Plant.herbariumId);
            if (dialog.ShowDialog() != true)
                return;

            var targetPlantId = dialog.TargetPlantId;

            var result = await PlantsService.Instance.MovePhotoAsync(
                Plant.herbariumId,
                Plant.id,
                photo.Metadata.id,
                targetPlantId);

            if (!result.Success)
            {
                MessageBox.Show("Failed to move photo.");
                return;
            }

            Photos.Remove(photo);
        }
    }
}
