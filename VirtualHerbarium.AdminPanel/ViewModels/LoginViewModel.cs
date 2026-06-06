using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Linq;
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
        private readonly LoginView _window;

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

        public LoginViewModel(LoginView window)
        {
            _window = window;

            LoginCommand = new RelayCommand(
            async () => await LoginAsync(),
            () => !IsBusy
            );

            ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguage);
        }

        private void ChangeLanguage(string lang)
        {
            LanguageManager.SetLanguage(lang);
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
                ErrorMessage = "Too many failed attempts. Try again in 10 seconds.";
                IsBusy = false;
                return;
            }

            if (result?.message == "NETWORK_ERROR")
            {
                ErrorMessage = "No connection to the server.";
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

            _window.OnLoginSuccess();

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
