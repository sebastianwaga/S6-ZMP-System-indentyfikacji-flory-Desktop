using System.Windows;
using System.Windows.Threading;
using VirtualHerbarium.AdminPanel.Services;
using VirtualHerbarium.AdminPanel.Views;

namespace VirtualHerbarium.AdminPanel
{
    public partial class App : Application
    {
        private DispatcherTimer _idleTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            _idleTimer = new DispatcherTimer();
            _idleTimer.Interval = TimeSpan.FromMinutes(5);
            _idleTimer.Tick += IdleTimer_Tick;
            _idleTimer.Start();

            EventManager.RegisterClassHandler(typeof(Window),
                UIElement.PreviewMouseDownEvent,
                new RoutedEventHandler(ResetIdleTimer));

            EventManager.RegisterClassHandler(typeof(Window),
                UIElement.PreviewKeyDownEvent,
                new RoutedEventHandler(ResetIdleTimer));

            var loginWindow = new LoginView();
            loginWindow.Show();
        }

        private void IdleTimer_Tick(object? sender, EventArgs e)
        {
            _idleTimer.Stop();

            AuthService.Instance.Logout();

            var main = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();

            main?.Close();

            new LoginView().Show();
        }

        private void ResetIdleTimer(object sender, RoutedEventArgs e)
        {
            _idleTimer.Stop();
            _idleTimer.Start();
        }
    }
}
