using System.Windows.Media.Imaging;

namespace VirtualHerbarium.AdminPanel.Models
{
    public class PlantPhotoResponse
    {
        public string id { get; set; }
        public string plantId { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public double confidence { get; set; }
        public DateTime createdAt { get; set; }

        public BitmapImage Image { get; set; }
    }
}
