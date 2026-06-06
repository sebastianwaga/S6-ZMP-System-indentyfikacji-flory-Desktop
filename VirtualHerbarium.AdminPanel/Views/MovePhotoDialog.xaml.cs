using System.Collections.Generic;
using System.Windows;
using VirtualHerbarium.AdminPanel.Models;
using VirtualHerbarium.AdminPanel.Services;

namespace VirtualHerbarium.AdminPanel.Views
{
    public partial class MovePhotoDialog : Window
    {
        public string TargetPlantId { get; private set; }

        public MovePhotoDialog(string herbariumId)
        {
            InitializeComponent();
            LoadPlants(herbariumId);
        }

        private async Task LoadPlants(string herbariumId)
        {
            var result = await PlantsService.Instance.GetPlantsAsync();

            if (!result.Success || result.Data == null)
            {
                MessageBox.Show("Nie udało się pobrać listy roślin.");
                Close();
                return;
            }

            var filtered = result.Data.FindAll(p => p.herbariumId == herbariumId);
            PlantsList.ItemsSource = filtered;
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            if (PlantsList.SelectedItem is PlantResponse plant)
            {
                TargetPlantId = plant.id;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Wybierz roślinę.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
