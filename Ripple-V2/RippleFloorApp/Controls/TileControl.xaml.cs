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
    /// Interaction logic for TileControl.xaml
    /// </summary>
    public partial class TileControl : UserControl
    {
        public TileControl()
        {
            InitializeComponent();
        }

        public Color TileBackground
        {
            set { this.TileID.Background = new SolidColorBrush(value); }
        }

        public double TileWidth
        {
            set { this.TileID.Height = value * RippleCommonUtilities.Globals.CurrentResolution.VerticalResolution; }
        }

        public double TileHeight
        {
            set { this.TileID.Width = value * RippleCommonUtilities.Globals.CurrentResolution.HorizontalResolution; }
        }

        public void SetMargin(double left, double top)
        {
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.TileID.Margin = new Thickness(0, left * RippleCommonUtilities.Globals.CurrentResolution.VerticalResolution, top * RippleCommonUtilities.Globals.CurrentResolution.HorizontalResolution, 0);
        }

        public String TileIDName
        {
            get { return this.TileID.Name; }
            set { this.TileID.Name = value; }
        }

        public String TileIDLabelName
        {
            get { return this.TileIDLabel.Name; }
            set { this.TileIDLabel.Name = value; }
        }

        public String InnerContentTileIDName
        {
            get { return this.InnerContentTileID.Name; }
            set { this.InnerContentTileID.Name = value; }
        }

        public String TileIDLabelText
        {
            get { return this.TileIDLabel.Text; }
            set { this.TileIDLabel.Text = value; }
        }
    }
}
