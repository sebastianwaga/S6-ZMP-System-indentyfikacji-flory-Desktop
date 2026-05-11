using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualHerbarium.AdminPanel.Models
{
    public class PlantResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string species { get; set; }
        public string owner { get; set; }
        public bool verified { get; set; }
    }
}
