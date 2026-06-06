using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VirtualHerbarium.AdminPanel.Services
{
    public class InternetService
    {
        public static InternetService Instance { get; } = new InternetService();

        private readonly HttpClient _http;
        private readonly DispatcherTimer _timer;

        public bool IsOnline { get; private set; } = true;

        public event Action<bool>? InternetStatusChanged;

        private InternetService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://ezielnik-production.up.railway.app/")
            };

            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += async (_, __) => await CheckConnection().ConfigureAwait(false);
            _timer.Start();
        }

        public async Task ForceCheck()
        {
            await CheckConnection();
        }

        private async Task CheckConnection()
        {
            try
            {

                var response = await _http.GetAsync("herbaria/public");

                bool newStatus = response.IsSuccessStatusCode;

                if (newStatus != IsOnline)
                {
                    IsOnline = newStatus;
                    InternetStatusChanged?.Invoke(IsOnline);
                }
            }
            catch
            {
                if (IsOnline)
                {
                    IsOnline = false;
                    InternetStatusChanged?.Invoke(false);
                }
            }
        }
    }
}
