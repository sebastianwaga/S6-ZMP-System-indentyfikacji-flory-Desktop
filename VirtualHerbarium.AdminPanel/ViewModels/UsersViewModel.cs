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
    public class UsersViewModel : INotifyPropertyChanged
    {
        private readonly UsersService _service;

        public ObservableCollection<AdminUserResponse> Users { get; set; }

        public ICommand BanCommand { get; }
        public ICommand UnbanCommand { get; }
        public ICommand MakeAdminCommand { get; }
        public ICommand RemoveAdminCommand { get; }
        public ICommand WarningCommand { get; }
        public ICommand ShowDetailsCommand { get; }
        public ICommand ShowFriendsCommand { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public UsersViewModel()
        {
            _service = new UsersService();
            Users = new ObservableCollection<AdminUserResponse>();

            BanCommand = new RelayCommand<AdminUserResponse>(async u => await BanUser(u));
            UnbanCommand = new RelayCommand<AdminUserResponse>(async u => await UnbanUser(u));
            MakeAdminCommand = new RelayCommand<AdminUserResponse>(async u => await MakeAdmin(u));
            RemoveAdminCommand = new RelayCommand<AdminUserResponse>(async u => await RemoveAdmin(u));
            WarningCommand = new RelayCommand<AdminUserResponse>(async u => await SendWarning(u));

            ShowDetailsCommand = new RelayCommand<AdminUserResponse>(async u => await ShowDetails(u));
            ShowFriendsCommand = new RelayCommand<AdminUserResponse>(async u => await ShowFriends(u));

            LoadUsers();
        }

        private async void LoadUsers()
        {
            IsBusy = true;

            var result = await _service.GetUsersAsync();

            if (!result.Success || result.Data == null)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }

            Users = new ObservableCollection<AdminUserResponse>(result.Data);
            OnPropertyChanged(nameof(Users));

            IsBusy = false;
        }

        private async Task BanUser(AdminUserResponse user)
        {
            IsBusy = true;

            var result = await _service.BanUserAsync(user.id);

            if (!result.Success)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }

            user.active = false;
            OnPropertyChanged(nameof(Users));

            MessageBox.Show(AppResources.Users_Success_Banned,
                AppResources.Success_Title, MessageBoxButton.OK, MessageBoxImage.Information);

            IsBusy = false;
        }

        private async Task UnbanUser(AdminUserResponse user)
        {
            IsBusy = true;

            var result = await _service.UnbanUserAsync(user.id);

            if (!result.Success)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }

            user.active = true;
            OnPropertyChanged(nameof(Users));

            MessageBox.Show(AppResources.Users_Success_Unbanned,
                AppResources.Success_Title, MessageBoxButton.OK, MessageBoxImage.Information);

            IsBusy = false;
        }

        private async Task MakeAdmin(AdminUserResponse user)
        {
            IsBusy = true;

            var result = await _service.MakeAdminAsync(user.id);

            if (!result.Success)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }

            user.admin = true;
            OnPropertyChanged(nameof(Users));

            MessageBox.Show(AppResources.Users_Success_MadeAdmin,
                AppResources.Success_Title, MessageBoxButton.OK, MessageBoxImage.Information);

            IsBusy = false;
        }

        private async Task RemoveAdmin(AdminUserResponse user)
        {
            IsBusy = true;

            var result = await _service.RemoveAdminAsync(user.id);

            if (!result.Success)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }

            user.admin = false;
            OnPropertyChanged(nameof(Users));

            MessageBox.Show(AppResources.Users_Success_RemovedAdmin,
                AppResources.Success_Title, MessageBoxButton.OK, MessageBoxImage.Information);

            IsBusy = false;
        }

        private async Task SendWarning(AdminUserResponse user)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "Wpisz treść ostrzeżenia:",
                "Wyślij ostrzeżenie",
                "Uwaga! Naruszenie regulaminu.");

            if (string.IsNullOrWhiteSpace(input))
                return;

            IsBusy = true;

            var success = await _service.SendWarningAsync(user.id, user.username, input);

            if (!success)
            {
                ShowError(500, "Warning failed");
                IsBusy = false;
                return;
            }

            MessageBox.Show(AppResources.Users_Success_WarningSent,
                AppResources.Success_Title, MessageBoxButton.OK, MessageBoxImage.Information);

            IsBusy = false;
        }


        private async Task ShowDetails(AdminUserResponse user)
        {
            IsBusy = true;

            var result = await _service.GetUserDetailsAsync(user.id);

            if (!result.Success || result.Data == null)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }
            IsBusy = false;

            var window = new UserDetailsView
            {
                DataContext = new UserDetailsViewModel(result.Data)
            };

            window.ShowDialog();

        }


        private async Task ShowFriends(AdminUserResponse user)
        {
            IsBusy = true;

            var result = await _service.GetUserFriendsAsync(user.id);

            if (!result.Success || result.Data == null)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }
            IsBusy = false;

            var window = new UserFriendsView
            {
                DataContext = new UserFriendsViewModel(result.Data)
            };

            window.ShowDialog();

        }

        private void ShowError(int code, string? error)
        {
            string msg = code switch
            {
                401 => AppResources.Error_SessionExpired,
                403 => AppResources.Error_NoPermission,
                404 => AppResources.Error_UserNotFound,
                _ => $"{AppResources.Error_Generic}: {error}"
            };

            MessageBox.Show(msg, AppResources.Error_Title,
                MessageBoxButton.OK, MessageBoxImage.Error);

            if (code == 401)
            {
                AuthService.Instance.Logout();
                Application.Current.Shutdown();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;

        public RelayCommand(Func<T, Task> execute) => _execute = execute;

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            if (parameter is T t)
                await _execute(t);
        }

        public event EventHandler? CanExecuteChanged;
    }
}
