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
    /// <summary>
    /// Interaction logic for MainOptionTile.xaml
    /// </summary>
    public partial class MainOptionTile : UserControl
    {
        public MainOptionTile(MainPage floorInstance)
        {
            InitializeComponent();
            this.MainOptionGrid.Visibility = System.Windows.Visibility.Collapsed;
            floorInstance.RegisterName(this.MainOptionGrid.Name, this.MainOptionGrid);
            floorInstance.RegisterName(this.MainOptionGridLabel.Name, this.MainOptionGridLabel);
        }

        public double ControlHeight
        {
            set { this.Height = value * Constants.VRatio;}
        }

        public double ControlWidth
        {
            set { this.Width = value * Constants.HRatio; }
        }

        public void SetMargin(double top, double height)
        {
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.Margin = new Thickness(50,(top - height) * Constants.VRatio + 50, 0,0);
        }

        public void UnregisterNames(MainPage floorInstance)
        {
            floorInstance.UnregisterName(this.MainOptionGrid.Name);
            floorInstance.UnregisterName(this.MainOptionGridLabel.Name);
        }

    }
}
