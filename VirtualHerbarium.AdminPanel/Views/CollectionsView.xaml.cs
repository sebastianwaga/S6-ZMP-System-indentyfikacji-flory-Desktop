using System.Windows.Controls;
using VirtualHerbarium.AdminPanel.ViewModels;

namespace VirtualHerbarium.AdminPanel.Views
{
    public partial class CollectionsView : UserControl
    {
        public CollectionsView()
        {
            InitializeComponent();
            DataContext = new CollectionsViewModel();
        }
    }
}
