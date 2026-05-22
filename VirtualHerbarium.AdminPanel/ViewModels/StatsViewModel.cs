using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class StatsViewModel : INotifyPropertyChanged
    {
        private readonly StatsService _service = new StatsService();

        private int _users;
        public int Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        private int _plants;
        public int Plants
        {
            get => _plants;
            set { _plants = value; OnPropertyChanged(); }
        }

        private int _collections;
        public int Collections
        {
            get => _collections;
            set { _collections = value; OnPropertyChanged(); }
        }

        public StatsViewModel()
        {
            LoadStats();
        }

        private async void LoadStats()
        {

            var result = await _service.GetStatsAsync();

            if (!result.Success || result.Data == null)
                return;

            Users = result.Data.users;
            Plants = result.Data.plants;
            Collections = result.Data.collections;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
