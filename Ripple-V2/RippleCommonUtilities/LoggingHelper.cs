using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleCommonUtilities
{
    public static class LoggingHelper
    {
        public static void StartLogging(String componentName, String fileLocation=null)
        {
            MicrosoftIT.ManagedLogging.LogManager.StartLogging(componentName, fileLocation);
        }

        public static void StopLogging()
        {
            MicrosoftIT.ManagedLogging.LogManager.StopLogging();
        }

        public static void LogTrace(int level, string formatString, params object[] varargs)
        {
            MicrosoftIT.ManagedLogging.LogManager.LogTrace(level, formatString, varargs);
        }
    }
}
