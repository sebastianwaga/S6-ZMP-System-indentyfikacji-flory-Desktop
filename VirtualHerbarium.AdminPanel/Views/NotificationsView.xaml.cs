using System.Windows.Controls;
using VirtualHerbarium.AdminPanel.ViewModels;

namespace VirtualHerbarium.AdminPanel.Views
{
    public partial class NotificationsView : UserControl
    {
        public NotificationsView()
        {
            InitializeComponent();
            DataContext = new NotificationsViewModel();
        }
    }
}
