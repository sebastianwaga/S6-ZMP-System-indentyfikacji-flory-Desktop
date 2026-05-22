using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VirtualHerbarium.AdminPanel.Helpers;
using VirtualHerbarium.AdminPanel.Services;
using VirtualHerbarium.AdminPanel.Views;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _auth = AuthService.Instance;
        private readonly Window _window;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        private string _login = string.Empty;
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand ChangeLanguageCommand { get; }

        public LoginViewModel(Window window)
        {
            _window = window;

            LoginCommand = new AsyncCommand(LoginAsync);
            ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguage);
        }

        private Task ChangeLanguage(string lang)
        {
            LanguageManager.SetLanguage(lang);
            return Task.CompletedTask;
        }

        private async Task LoginAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ErrorMessage = "";

            var result = await _auth.LoginAsync(Login, Password);

            if (result?.message == "LOCKED")
            {
                ErrorMessage = "Zbyt wiele nieudanych prób. Spróbuj ponownie za 10 sekund.";
                IsBusy = false;
                return;
            }

            if (result?.message == "NETWORK_ERROR")
            {
                ErrorMessage = "Brak połączenia z serwerem.";
                IsBusy = false;
                return;
            }

            if (result == null)
            {
                ErrorMessage = AppResources.Login_Error_InvalidCredentials;
                IsBusy = false;
                return;
            }

            if (!result.admin)
            {
                ErrorMessage = AppResources.Login_Error_NoAdminRights;
                IsBusy = false;
                return;
            }

            var main = new MainWindow();
            main.Show();
            _window.Close();

            IsBusy = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private bool _isExecuting;

        public AsyncCommand(Func<Task> execute) => _execute = execute;

        public bool CanExecute(object? parameter) => !_isExecuting;

        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            try { await _execute(); }
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}
