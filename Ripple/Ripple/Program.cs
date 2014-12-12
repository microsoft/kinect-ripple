using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace Ripple
{
    //Launches both the applications
    class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    //Stop the logging
                    RippleCommonUtilities.LoggingHelper.StopLogging();
                    break;
                default:
                    break;
            }
            return true;
        }

        static void Main(string[] args)
        {
            //Handler to handle close window events
            //_handler += new EventHandler(Handler);
            //SetConsoleCtrlHandler(_handler, true);

            //Attach global exception handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //Start the logging
            //RippleCommonUtilities.LoggingHelper.StartLogging("RippleApp");

            var screenList = Screen.AllScreens;

            //Check whether only 2 displays connected
            Console.WriteLine("Checking for connected displays...\r\n");
            if (screenList.Length < 2)
            {
                Console.WriteLine("There should at least be 2 displays connected. \r\n");
                Console.WriteLine("Press enter to exit\r\n");
                Console.Read();
                return;
            }

            Console.WriteLine("Displays verified successfully\r\n\r\n");


            //Get the resolution for both the displays
            Console.WriteLine("Checking for resolution for the displays...\r\n");
            String sample = String.Empty;

            //Get individual resolutions
            RippleCommonUtilities.RippleScreenResoultion FloorAppResolution = new RippleCommonUtilities.RippleScreenResoultion();
            RippleCommonUtilities.RippleScreenResoultion ScreenAppResolution = new RippleCommonUtilities.RippleScreenResoultion();
            
            foreach (var scr in screenList)
            {
                //It means its the floor
                if (scr.Primary)
                {
                    FloorAppResolution.ScreenName = scr.DeviceName;
                    FloorAppResolution.HorizontalResolution = SystemParameters.PrimaryScreenWidth;
                    FloorAppResolution.VerticalResolution = SystemParameters.PrimaryScreenHeight;
                    FloorAppResolution.XOrigin = scr.Bounds.Left;
                    FloorAppResolution.YOrigin = scr.Bounds.Top;
                }
                //It means its the screen, since only 2 displays are connected
                else
                {
                    ScreenAppResolution.ScreenName = scr.DeviceName;
                    ScreenAppResolution.HorizontalResolution = SystemParameters.VirtualScreenWidth - SystemParameters.PrimaryScreenWidth;
                    ScreenAppResolution.VerticalResolution = SystemParameters.VirtualScreenHeight;
                    ScreenAppResolution.XOrigin = SystemParameters.PrimaryScreenWidth;
                    ScreenAppResolution.YOrigin = scr.Bounds.Top;
                }
                sample += String.Format("DeviceName: {0} \r\nBounding Rectangle: W{1} h{2}\r\n", scr.DeviceName, scr.Bounds.Width, scr.Bounds.Height);
            }

            //Check whether the primary screen has 0,0 origin
            if (!(FloorAppResolution.XOrigin == 0 && FloorAppResolution.YOrigin == 0))
            {
                Console.WriteLine("Please set the primary screen with origin 0,0 by moving it to the left most in the Screen Resolution window for the machine\r\n");
                Console.WriteLine("Press enter to exit");
                Console.Read();
                return;
            }

            Console.WriteLine(sample);
            Console.WriteLine("Resolution verified successfully\r\n\r\n");

            Console.WriteLine("Starting, Please wait a moment");
            System.Threading.Thread.Sleep(1000);
            
            //Project floor on the primary display
            Console.WriteLine("Starting Floor Application on the primary screen...\r\n");
            StartFloorApplication(FloorAppResolution.YOrigin, FloorAppResolution.XOrigin, FloorAppResolution.VerticalResolution, FloorAppResolution.HorizontalResolution);

            //Project screen on the secondary display
            Console.WriteLine("Starting Screen Application on the primary screen...\r\n");
            StartScreenApplication(ScreenAppResolution.YOrigin, ScreenAppResolution.XOrigin, ScreenAppResolution.VerticalResolution, ScreenAppResolution.HorizontalResolution);

            //Confirm with the user if he is happy, else ask him to make floor projection as the primary display and make it the left most.
            Console.WriteLine("Are you happy with the projection setup? y:n\r\n");
            //char happyState = 'n';
            return;
            //if (happyState == 'y')
            //{
            //    Console.WriteLine("We are happy that you liked the setup, have fun playing around\r\n");
            //    Console.WriteLine("Press enter to exit\r\n");
            //    Console.Read();
            //    return;
            //}
            //else
            //{
            //    //TODO - record the feedback
            //    Console.WriteLine("We would improve the setup further, thanks a lot for your feedback\r\n");
            //    Console.WriteLine("Press enter to exit\r\n");
            //    Console.Read();
            //    return;
            //}
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Do nothing
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.Read();
        }

        private static void StartFloorApplication(double top, double left, double vRes, double hRes)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\RippleFloor\\RippleFloorApp.exe";
                startInfo.Arguments = "/Top " + top.ToString() + " /Left " + left.ToString() + " /VRes " + vRes.ToString() + " /HRes " + hRes.ToString();
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Went wrong in Starting floor application " + ex.Message);
                Console.Read();
            }
        }

        private static void StartScreenApplication(double top, double left, double vRes, double hRes)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\RippleScreen\\RippleScreenApp.exe";
                startInfo.Arguments = "/Top " + top.ToString() + " /Left " + left.ToString() + " /VRes " + vRes.ToString() + " /HRes " + hRes.ToString();
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Went wrong in Starting screen application " + ex.Message);
                Console.Read();
            }
        }
    }
}
