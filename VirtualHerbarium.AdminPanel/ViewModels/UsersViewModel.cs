using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VirtualHerbarium.AdminPanel.Models;
// using VirtualHerbarium.AdminPanel.Services;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class UsersViewModel : INotifyPropertyChanged
    {
        // private readonly UsersService _service;
        private readonly bool _useMock = true;

        public ObservableCollection<AdminUserResponse> Users { get; set; }

        public ICommand BanCommand { get; }
        public ICommand UnbanCommand { get; }
        public ICommand MakeAdminCommand { get; }
        public ICommand RemoveAdminCommand { get; }
        public ICommand WarningCommand { get; }

        public UsersViewModel()
        {
            Users = new ObservableCollection<AdminUserResponse>();

            BanCommand = new RelayCommand<AdminUserResponse>(async u => await BanUser(u));
            UnbanCommand = new RelayCommand<AdminUserResponse>(async u => await UnbanUser(u));
            MakeAdminCommand = new RelayCommand<AdminUserResponse>(async u => await MakeAdmin(u));
            RemoveAdminCommand = new RelayCommand<AdminUserResponse>(async u => await RemoveAdmin(u));
            WarningCommand = new RelayCommand<AdminUserResponse>(async u => await SendWarning(u));

            LoadUsers();
        }

        private async void LoadUsers()
        {
            if (_useMock)
            {
                Users = new ObservableCollection<AdminUserResponse>
                {
                    new AdminUserResponse { email="test1@example.com", username="user1", active=true, verified=true, admin=false },
                    new AdminUserResponse { email="test2@example.com", username="user2", active=false, verified=true, admin=false },
                    new AdminUserResponse { email="admin@example.com", username="admin", active=true, verified=true, admin=true }
                };
            }
            else
            {
                // var result = await _service.GetUsersAsync();
                // if (!result.Success) { ShowError(result); return; }
                // Users = new ObservableCollection<AdminUserResponse>(result.Data);
            }

            OnPropertyChanged(nameof(Users));
        }

        private async Task BanUser(AdminUserResponse user)
        {
            if (!_useMock)
            {
                // var result = await _service.BanUserAsync(user.id);
                // if (!result.Success) { ShowError(result); return; }
            }

            user.active = false;
            OnPropertyChanged(nameof(Users));
        }

        private async Task UnbanUser(AdminUserResponse user)
        {
            if (!_useMock)
            {
                // var result = await _service.UnbanUserAsync(user.id);
                // if (!result.Success) { ShowError(result); return; }
            }

            user.active = true;
            OnPropertyChanged(nameof(Users));
        }

        private async Task MakeAdmin(AdminUserResponse user)
        {
            if (!_useMock)
            {
                // var result = await _service.MakeAdminAsync(user.id);
                // if (!result.Success) { ShowError(result); return; }
            }

            user.admin = true;
            OnPropertyChanged(nameof(Users));
        }

        private async Task RemoveAdmin(AdminUserResponse user)
        {
            if (!_useMock)
            {
                // var result = await _service.RemoveAdminAsync(user.id);
                // if (!result.Success) { ShowError(result); return; }
            }

            user.admin = false;
            OnPropertyChanged(nameof(Users));
        }

        private async Task SendWarning(AdminUserResponse user)
        {
            if (_useMock)
            {
                System.Windows.MessageBox.Show("Mock: wysłano ostrzeżenie");
                return;
            }

            // var result = await _service.SendWarningAsync(user.id, "Uwaga! Naruszenie regulaminu.");
            // if (!result.Success) { ShowError(result); return; }

            System.Windows.MessageBox.Show("Ostrzeżenie wysłane.");
        }

        private void ShowError(ApiResult<bool> result)
        {
            string msg = result.StatusCode switch
            {
                401 => "Sesja wygasła. Zaloguj się ponownie.",
                403 => "Brak uprawnień.",
                404 => "Użytkownik nie istnieje.",
                500 => "Błąd serwera.",
                _ => $"Błąd: {result.Error}"
            };

            System.Windows.MessageBox.Show(msg, "Błąd API",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
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
