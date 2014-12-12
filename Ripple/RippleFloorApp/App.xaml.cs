using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RippleFloorApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            String fileLocation = System.Configuration.ConfigurationManager.AppSettings["LogFileLocation"];

            if (!String.IsNullOrEmpty(fileLocation))
            {
                RippleCommonUtilities.LoggingHelper.StartLogging("RippleApp", fileLocation);
            }
            else
            {
                RippleCommonUtilities.LoggingHelper.StartLogging("RippleApp");
            }

            double top = 0.0;
            double left = 0.0;
            double HRes = 1280;
            double VRes = 800;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/Top")
                {
                    top = Convert.ToDouble(e.Args[++i]);
                }
                else if (e.Args[i] == "/Left")
                {
                    left = Convert.ToDouble(e.Args[++i]);
                }
                else if (e.Args[i] == "/VRes")
                {
                    VRes = Convert.ToDouble(e.Args[++i]);
                }
                else if (e.Args[i] == "/HRes")
                {
                    HRes = Convert.ToDouble(e.Args[++i]);
                }
            }

            //Set the globals
            RippleCommonUtilities.Globals.CurrentResolution.VerticalResolution = VRes;
            RippleCommonUtilities.Globals.CurrentResolution.HorizontalResolution = HRes;
            RippleCommonUtilities.Globals.CurrentResolution.XOrigin = left;
            RippleCommonUtilities.Globals.CurrentResolution.YOrigin = top;
            // Create main application window
            FloorWindow floorWin = new FloorWindow();
            floorWin.Top = top;
            floorWin.Left = left;
            floorWin.Topmost = true;
            floorWin.BorderThickness = new Thickness(0.2);
            floorWin.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0072C6"));
            floorWin.WindowStartupLocation = WindowStartupLocation.Manual;
            floorWin.Width = HRes;
            floorWin.Height = VRes;
            //floorWin.Width = 800;
            //floorWin.Height = 500;
            floorWin.WindowState = WindowState.Maximized;
            floorWin.WindowStyle = WindowStyle.None;
            floorWin.ResizeMode = ResizeMode.NoResize;
            floorWin.Show();
        }

        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            //Stop the logging session
            RippleCommonUtilities.LoggingHelper.StopLogging();
        }

        private void Application_DispatcherUnhandledException_1(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //Stop the logging session
            RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Floor {0}", e.Exception.Message);
            //RippleCommonUtilities.LoggingHelper.StopLogging();
            e.Handled = true;
        }
    }
}
