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
    public class CollectionsViewModel : INotifyPropertyChanged
    {
        private readonly CollectionsService _service = new CollectionsService();

        public ObservableCollection<CollectionResponse> Collections { get; set; }

        public ICommand DeleteCommand { get; }

        public CollectionsViewModel()
        {
            Collections = new ObservableCollection<CollectionResponse>();
            DeleteCommand = new RelayCommand<CollectionResponse>(async c => await DeleteCollection(c));

            LoadCollections();
        }

        private async void LoadCollections()
        {
            var result = await _service.GetCollectionsAsync();

            if (!result.Success || result.Data == null)
            {
                ShowError(AppResources.Error_FetchCollections);
                return;
            }

            Collections = new ObservableCollection<CollectionResponse>(result.Data);
            OnPropertyChanged(nameof(Collections));
        }

        private async Task DeleteCollection(CollectionResponse collection)
        {
            var confirm = MessageBox.Show(
                string.Format(AppResources.Confirm_DeleteCollection, collection.name),
                AppResources.Confirm_Title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            var result = await _service.DeleteCollectionAsync(collection.id);

            if (!result.Success)
            {
                ShowError(AppResources.Error_DeleteCollection);
                return;
            }

            Collections.Remove(collection);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, AppResources.Error_Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
