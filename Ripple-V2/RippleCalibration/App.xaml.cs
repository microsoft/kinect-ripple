using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RippleCalibration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create main application window
            MainWindow floorWin = new MainWindow();
            floorWin.Top = 0.0;
            floorWin.Left = 0.0;
            floorWin.Topmost = true;
            floorWin.BorderThickness = new Thickness(0.2);
            floorWin.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0072C6"));
            floorWin.WindowStartupLocation = WindowStartupLocation.Manual;
            floorWin.Width = SystemParameters.PrimaryScreenWidth;
            floorWin.Height = SystemParameters.PrimaryScreenHeight;
            floorWin.WindowState = WindowState.Maximized;
            floorWin.WindowStyle = WindowStyle.None;
            floorWin.ResizeMode = ResizeMode.NoResize;
            floorWin.Show();
        }
    }
}
