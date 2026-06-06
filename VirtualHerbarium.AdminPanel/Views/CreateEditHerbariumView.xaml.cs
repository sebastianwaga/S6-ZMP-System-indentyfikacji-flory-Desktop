using System.Windows;

namespace VirtualHerbarium.AdminPanel.Views
{
    public partial class CreateEditHerbariumView : Window
    {
        public CreateEditHerbariumView()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
