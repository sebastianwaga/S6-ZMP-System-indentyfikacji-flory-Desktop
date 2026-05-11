using System.Configuration;
using System.Data;
using System.Windows;
using VirtualHerbarium.AdminPanel.Views;

namespace VirtualHerbarium.AdminPanel
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = new LoginView();
            new MainWindow().Show();
            loginWindow.Show();
        }
    }
}
