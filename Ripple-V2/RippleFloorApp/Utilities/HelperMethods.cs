using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleFloorApp.Utilities
{
    public static class HelperMethods
    {
        public static string GetAssetURI(string Content)
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\.." + Content;
        }
    }
}
