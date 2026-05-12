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
        private readonly NotificationsService _service = new NotificationsService();

        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public ICommand SendCommand { get; }

        public NotificationsViewModel()
        {
            SendCommand = new RelayCommand<object>(async _ => await Send());
        }

        private async Task Send()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Message))
            {
                MessageBox.Show(
                    AppResources.Notifications_Error_EmptyFields,
                    AppResources.Error_Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var request = new NotificationRequest
            {
                title = Title,
                message = Message
            };

            var result = await _service.SendNotificationAsync(request);

            if (!result.Success)
            {
                MessageBox.Show(
                    AppResources.Notifications_Error_SendFailed,
                    AppResources.Error_Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            MessageBox.Show(
                AppResources.Notifications_Success_Sent,
                AppResources.Success_Title,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            Title = string.Empty;
            Message = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
