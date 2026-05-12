using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualHerbarium.AdminPanel.Models
{
    public class CollectionResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public int plantCount { get; set; }
        public bool shared { get; set; }
    }
}
