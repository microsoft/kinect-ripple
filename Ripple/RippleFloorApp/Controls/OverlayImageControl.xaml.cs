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
    /// Interaction logic for OverlayImageControl.xaml
    /// </summary>
    public partial class OverlayImageControl : UserControl
    {
        public OverlayImageControl(FloorWindow floorInstance)
        {
            InitializeComponent();
            floorInstance.RegisterName(this.OverlayImage.Name, this.OverlayImage);
        }

        public void SetMargin(double tileHeight)
        {
            this.Margin = new Thickness((2 * tileHeight * RippleCommonUtilities.Globals.CurrentResolution.HorizontalResolution + Utilities.Constants.OverlayImageMargin), Utilities.Constants.OverlayImageMargin, (tileHeight * RippleCommonUtilities.Globals.CurrentResolution.HorizontalResolution + Utilities.Constants.OverlayImageMargin), Utilities.Constants.OverlayImageMargin);
        }
    }
}
