using System.Windows;
using System.Windows.Controls;
using VirtualHerbarium.AdminPanel.Services;
using VirtualHerbarium.AdminPanel.ViewModels;

namespace VirtualHerbarium.AdminPanel.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }

        public async void OnLoginSuccess()
        {
            var main = new MainWindow();
            main.Show();

            SyncWorker.Instance.Start();

            await InternetService.Instance.ForceCheck();

            Close();
        }
    }
}
