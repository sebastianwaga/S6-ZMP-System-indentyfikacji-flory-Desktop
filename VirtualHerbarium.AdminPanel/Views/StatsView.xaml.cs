using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using VirtualHerbarium.AdminPanel.ViewModels;

namespace VirtualHerbarium.AdminPanel.Views
{
    public partial class StatsView : UserControl
    {
        public StatsView()
        {
            InitializeComponent();
            DataContext = new StatsViewModel();
        }
    }
}
