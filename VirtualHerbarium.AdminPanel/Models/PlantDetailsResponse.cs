namespace VirtualHerbarium.AdminPanel.Models
{
    public class PlantDetailsResponse
    {
        public string id { get; set; }
        public string herbariumId { get; set; }
        public string name { get; set; }
        public string detectedSpecies { get; set; }
        public string speciesId { get; set; }
        public string family { get; set; }
        public string genus { get; set; }
        public string commonNames { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        public List<PlantPhotoResponse> photos { get; set; }
    }
}
