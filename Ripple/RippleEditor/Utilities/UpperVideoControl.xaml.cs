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

namespace RippleEditor.Utilities
{
    // <summary>
    /// Interaction logic for UpperVideoControl.xaml
    /// </summary>
    public partial class UpperVideoControl : UserControl
    {
        public UpperVideoControl(MainPage floorInstance)
        {
            InitializeComponent();

            //Register the names
            floorInstance.RegisterName(this.UpperTile.Name, this.UpperTile);
            floorInstance.RegisterName(this.FloorVideoControl.Name, this.FloorVideoControl);
        }

        public double ControlHeight
        {
            set { this.Height = value * Constants.VRatio; }
        }

        public double ControlWidth
        {
            set { this.Width = value * Constants.HRatio; }
        }

        public void SetMargin(double left, double top)
        {
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.Margin = new Thickness(left * Constants.HRatio + 50, top * Constants.VRatio + 50, 0, 0);
        }

        public void UnregisterNames(MainPage floorInstance)
        {
            floorInstance.UnregisterName(this.UpperTile.Name);
            floorInstance.UnregisterName(this.FloorVideoControl.Name);
        }
    }
}
