using System;
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
        private readonly UsersService _service = UsersService.Instance;

        public ObservableCollection<AdminUserResponse> Users { get; private set; }
            = new ObservableCollection<AdminUserResponse>();

        public ICommand BanCommand { get; }
        public ICommand UnbanCommand { get; }
        public ICommand MakeAdminCommand { get; }
        public ICommand RemoveAdminCommand { get; }
        public ICommand WarningCommand { get; }
        public ICommand ShowDetailsCommand { get; }
        public ICommand ShowFriendsCommand { get; }
        public ICommand DeleteUserCommand { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public UsersViewModel()
        {

            BanCommand = new AsyncRelayCommand<AdminUserResponse>(BanUser);
            UnbanCommand = new AsyncRelayCommand<AdminUserResponse>(UnbanUser);
            MakeAdminCommand = new AsyncRelayCommand<AdminUserResponse>(MakeAdmin);
            RemoveAdminCommand = new AsyncRelayCommand<AdminUserResponse>(RemoveAdmin);
            WarningCommand = new AsyncRelayCommand<AdminUserResponse>(SendWarning);
            ShowDetailsCommand = new AsyncRelayCommand<AdminUserResponse>(ShowDetails);
            ShowFriendsCommand = new AsyncRelayCommand<AdminUserResponse>(ShowFriends);
            DeleteUserCommand = new AsyncRelayCommand<AdminUserResponse>(DeleteUser);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            IsBusy = true;

            try
            {
                var result = await _service.GetUsersAsync();

                if (!result.Success || result.Data == null)
                {
                    ShowError(result.StatusCode, result.Error);
                    return;
                }

                Users.Clear();
                foreach (var u in result.Data)
                    Users.Add(u);

                OnPropertyChanged(nameof(Users));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading users:\n" + ex.Message);

            }
            finally
            {
                IsBusy = false;
            }
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
            "Enter warning message:",
            "Send warning",
            "Warning! Terms violation.");


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

        private async Task DeleteUser(AdminUserResponse user)
        {
            var confirm = MessageBox.Show(
            $"Are you sure you want to permanently delete user {user.username}?\nThis action is irreversible.",
            "Delete confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);


            if (confirm != MessageBoxResult.Yes)
                return;

            IsBusy = true;

            var result = await _service.DeleteUserAsync(user.id);

            if (!result.Success)
            {
                ShowError(result.StatusCode, result.Error);
                IsBusy = false;
                return;
            }

            Users.Remove(user);

            MessageBox.Show("User has been permanently deleted.",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);


            IsBusy = false;
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

    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;

        public AsyncRelayCommand(Func<T, Task> execute) => _execute = execute;

        public bool CanExecute(object? parameter) => parameter is T;

        public async void Execute(object? parameter)
        {
            if (parameter is T t)
                await _execute(t);
        }

        public event EventHandler? CanExecuteChanged;
    }
}
