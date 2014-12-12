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
using RippleEditor.Utilities;

namespace RippleEditor.Controls
{
    /// <summary>
    /// Interaction logic for OverlayImageControl.xaml
    /// </summary>
    public partial class OverlayImageControl : UserControl
    {
        public OverlayImageControl(MainPage main)
        {
            InitializeComponent();
            main.RegisterName(this.OverlayImage.Name, this.OverlayImage);
        }

        public void SetMargin(double tileHeight, double tileWidth)
        {
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.Width = tileWidth;
            this.Height = tileHeight;
            this.Margin = new Thickness(Constants.OverlayImageMargin + 50, (tileHeight * Constants.VRatio + Constants.OverlayImageMargin),0,0);
        }
    }
}
