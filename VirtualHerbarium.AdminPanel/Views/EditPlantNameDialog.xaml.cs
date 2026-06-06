using System.Windows;

namespace VirtualHerbarium.AdminPanel.Views
{
    public partial class EditPlantNameDialog : Window
    {
        public string NewName { get; private set; }

        public EditPlantNameDialog(string currentName)
        {
            InitializeComponent();
            NameBox.Text = currentName;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Wpisz nazwę.");
                return;
            }

            NewName = NameBox.Text;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
                DialogResult = false;

            base.OnClosing(e);
        }

    }
}
