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
    public class NotificationsViewModel : INotifyPropertyChanged
    {
        private readonly NotificationsService _service = NotificationsService.Instance;

        public ObservableCollection<NotificationResponse> Notifications { get; set; }
            = new ObservableCollection<NotificationResponse>();

        public ICommand RefreshCommand { get; }
        public ICommand MarkAllCommand { get; }
        public ICommand MarkReadCommand { get; }

        public NotificationsViewModel()
        {
            RefreshCommand = new RelayCommand<object>(async _ => await Load());
            MarkAllCommand = new RelayCommand<object>(async _ => await MarkAll());
            MarkReadCommand = new RelayCommand<NotificationResponse>(async n => await MarkOne(n));

            _ = Load();
        }

        private async Task Load()
        {
            var list = await _service.GetUnreadAsync();
            Notifications.Clear();

            if (list != null)
                foreach (var n in list)
                    Notifications.Add(n);
        }

        private async Task MarkOne(NotificationResponse n)
        {
            if (await _service.MarkAsReadAsync(n.id))
                await Load();
        }

        private async Task MarkAll()
        {
            if (await _service.MarkAllAsReadAsync())
                await Load();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
