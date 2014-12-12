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
    /// Interaction logic for UpperVideoControl.xaml
    /// </summary>
    public partial class UpperVideoControl : UserControl
    {
        public UpperVideoControl(FloorWindow floorInstance)
        {
            InitializeComponent();

            //Register the names
            floorInstance.RegisterName(this.UpperTile.Name, this.UpperTile);
            floorInstance.RegisterName(this.FloorVideoControl.Name, this.FloorVideoControl);
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
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.Margin = new Thickness(0, left * RippleCommonUtilities.Globals.CurrentResolution.VerticalResolution, top * RippleCommonUtilities.Globals.CurrentResolution.HorizontalResolution, 0);
        }
    }
}
