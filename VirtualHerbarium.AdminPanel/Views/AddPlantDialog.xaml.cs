using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace VirtualHerbarium.AdminPanel.Views
{
    public partial class AddPlantDialog : Window
    {
        public string PlantName { get; private set; }
        public string PhotoBase64 { get; private set; }
        public string HerbariumId { get; private set; }

        public AddPlantDialog()
        {
            InitializeComponent();
        }

        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Zdjęcia (*.jpg;*.png)|*.jpg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                PhotoPathBox.Text = dialog.FileName;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(PhotoPathBox.Text) ||
                string.IsNullOrWhiteSpace(HerbariumBox.Text))
            {
                MessageBox.Show("Wypełnij wszystkie pola.");
                return;
            }

            PlantName = NameBox.Text;
            HerbariumId = HerbariumBox.Text;

            var bytes = File.ReadAllBytes(PhotoPathBox.Text);
            PhotoBase64 = Convert.ToBase64String(bytes);

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
