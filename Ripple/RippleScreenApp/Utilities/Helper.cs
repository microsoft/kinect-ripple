using RippleCommonUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RippleScreenApp.Utilities
{
    public static class Helper
    {
        public static void ClickOnScreenToGetFocus()
        {
            int middleWidth = (int)SystemParameters.PrimaryScreenWidth + Convert.ToInt32(Math.Floor((double)((int)Globals.CurrentResolution.HorizontalResolution / 2)));
            int middleHeight = (int)SystemParameters.PrimaryScreenHeight + Convert.ToInt32(Math.Floor((double)((int)Globals.CurrentResolution.VerticalResolution / 2)));
            //RippleCommonUtilities.LoggingHelper.LogTrace(1, "Click on the screen middleW: {0} middleH: {1} width: {2} height: {3}", middleWidth, middleHeight, Globals.CurrentResolution.HorizontalResolution, Globals.CurrentResolution.VerticalResolution);
            RippleCommonUtilities.OSNativeMethods.SendMouseInput(middleWidth, middleHeight, (int)Globals.CurrentResolution.HorizontalResolution, (int)Globals.CurrentResolution.VerticalResolution, true);
            RippleCommonUtilities.OSNativeMethods.SendMouseInput(middleWidth, middleHeight, (int)Globals.CurrentResolution.HorizontalResolution, (int)Globals.CurrentResolution.VerticalResolution, false);
        }

        public static string GetAssetURI(string Content)
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\.." + Content;
        }
    }
}
