using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RippleFloorApp.Controls
{
    /// <summary>
    /// Interaction logic for MainOptionTile.xaml
    /// </summary>
    public partial class MainOptionTile : UserControl
    {
        public MainOptionTile(FloorWindow floorInstance)
        {
            InitializeComponent();
            floorInstance.RegisterName(this.MainOptionGrid.Name, this.MainOptionGrid);
            floorInstance.RegisterName(this.MainOptionGridLabel.Name, this.MainOptionGridLabel);
        }

        public double ControlWidth
        {
            set { this.Height = value * RippleCommonUtilities.Globals.CurrentResolution.VerticalResolution; }
        }

        public double ControlHeight
        {
            set { this.Width = value * RippleCommonUtilities.Globals.CurrentResolution.HorizontalResolution; }
        }

        public void SetMargin(double left, double top)
        {
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.Margin = new Thickness(left * RippleCommonUtilities.Globals.CurrentResolution.HorizontalResolution,0,0,0);
        }

    }
}
