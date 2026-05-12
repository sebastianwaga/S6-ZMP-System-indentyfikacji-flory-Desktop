using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualHerbarium.AdminPanel.Models
{
    public class StatsResponse
    {
        public int users { get; set; }
        public int plants { get; set; }
        public int collections { get; set; }
    }
}
