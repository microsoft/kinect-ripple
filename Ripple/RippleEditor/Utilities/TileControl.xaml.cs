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
            set { this.TileID.Width = value * Constants.HRatio; }
        }

        public double TileHeight
        {
            set { this.TileID.Height = value * Constants.VRatio; }
        }

        public void SetMargin(double left, double top)
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.TileID.Margin = new Thickness(left * Constants.HRatio + 50, top * Constants.VRatio + 50, 0, 0);
        }

        public void SetNames(String tileID, MainPage mainInstance)
        {
            TileIDName = TileIDName.Replace("TileID", tileID);
            InnerTileIDName = InnerTileIDName.Replace("TileID", tileID);
            TileIDLabelName = TileIDLabelName.Replace("TileID", tileID);
            InnerContentTileIDName = InnerContentTileIDName.Replace("TileID", tileID);
            TileIDButtonName = TileIDButtonName.Replace("TileID", tileID);

            mainInstance.RegisterName(TileIDName, TileID);
            mainInstance.RegisterName(InnerContentTileIDName, InnerContentTileID);
            mainInstance.RegisterName(TileIDLabelName, TileIDLabel);
            mainInstance.RegisterName(InnerTileIDName, InnerTileID);
            mainInstance.RegisterName(TileIDButtonName, TileIDButton);
        }

        public String TileIDName 
        {
            get { return this.TileID.Name; }
            set { this.TileID.Name = value; }
        }

        public String TileIDButtonName
        {
            get { return this.TileIDButton.Name; }
            set { this.TileIDButton.Name = value; }
        }

        public String TileIDLabelName
        {
            get { return this.TileIDLabel.Name; }
            set { this.TileIDLabel.Name = value; }
        }

        public String InnerTileIDName
        {
            get { return this.InnerTileID.Name; }
            set { this.InnerTileID.Name = value; }
        }

        public String InnerContentTileIDName
        {
            get { return this.InnerContentTileID.Name; }
            set { this.InnerContentTileID.Name = value; }
        }

        public String TileIDLabelText
        {
            get { return this.TileIDLabel.Name; }
            set { this.TileIDLabel.Text = value; }
        }

        public void UnregisterNames(MainPage mainInstance)
        {
            mainInstance.UnregisterName(TileIDName);
            mainInstance.UnregisterName(InnerContentTileIDName);
            mainInstance.UnregisterName(TileIDLabelName);
            mainInstance.UnregisterName(InnerTileIDName);
            mainInstance.UnregisterName(TileIDButtonName);
        }
    }
}
