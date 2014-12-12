using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RippleEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            //Stop the logging session
            RippleCommonUtilities.LoggingHelper.StopLogging();
        }

        private void Application_DispatcherUnhandledException_1(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //Stop the logging session
            RippleCommonUtilities.LoggingHelper.StopLogging();
            e.Handled = true;
        }
    }
}
