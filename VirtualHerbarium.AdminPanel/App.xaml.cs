using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using VirtualHerbarium.AdminPanel.Services;
using VirtualHerbarium.AdminPanel.Views;

namespace VirtualHerbarium.AdminPanel
{
    public partial class App : Application
    {
        private static Mutex? _mutex;
        private DispatcherTimer? _idleTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "VirtualHerbarium.AdminPanel.SingleInstance";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                Shutdown();
                return;
            }

            base.OnStartup(e);

            _idleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(5)
            };
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
            _idleTimer?.Stop();

            if (Application.Current.Windows.Count == 0)
            {
                Application.Current.Shutdown();
                return;
            }

            if (Application.Current.Windows.OfType<LoginView>().Any() &&
                !Application.Current.Windows.OfType<MainWindow>().Any())
            {
                _idleTimer?.Start();
                return;
            }

            AuthService.Instance.Logout();

            var main = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();

            main?.Close();

            if (Application.Current.Windows.Count > 0)
                new LoginView().Show();

            _idleTimer?.Start();
        }

        private void ResetIdleTimer(object sender, RoutedEventArgs e)
        {
            _idleTimer?.Stop();
            _idleTimer?.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _idleTimer?.Stop();
            _idleTimer = null;

            _mutex?.ReleaseMutex();
            _mutex = null;

            base.OnExit(e);
        }
    }
}
