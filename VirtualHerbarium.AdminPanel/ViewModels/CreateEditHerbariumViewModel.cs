using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class CreateEditHerbariumViewModel : INotifyPropertyChanged
    {
        public string Title { get; }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private bool _isPublic;
        public bool IsPublic
        {
            get => _isPublic;
            set { _isPublic = value; OnPropertyChanged(); }
        }


        public CreateEditHerbariumViewModel(string title, string name = "", string description = "", bool isPublic = false)
        {
            Title = title;
            Name = name;
            Description = description;
            IsPublic = isPublic;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
