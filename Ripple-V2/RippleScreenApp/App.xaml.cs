using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RippleScreenApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
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
            ScreenWindow screenWin = new ScreenWindow();
            screenWin.WindowStartupLocation = WindowStartupLocation.Manual;
            screenWin.Top = top;
            screenWin.Left = left;
            screenWin.BorderThickness = new Thickness(0.2);
            screenWin.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0072C6"));
            screenWin.WindowStyle = WindowStyle.None;
            screenWin.Height = VRes;
            screenWin.Width = HRes;
            screenWin.ResizeMode = ResizeMode.NoResize;
            screenWin.Show();
        }

        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            //Commit the telemetry data
            Utilities.TelemetryWriter.CommitTelemetry();
        }

        private void Application_DispatcherUnhandledException_1(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in screen {0}", e.Exception.Message);
            e.Handled = true;
        }
    }
}
