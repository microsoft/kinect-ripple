using System;
using System.Diagnostics;

namespace MicrosoftIT.ManagedLogging
{
    public static class LogManager
    {
        private static bool LoggingStarted = false;
        private static string ComponentCode = "Sample";
        private static String sessionName = "SampleSession";

        public static void StartLogging(String componentName, String fileLocation = "")
        {
            try
            {
                if (!LoggingStarted)
                {
                    String fileName = "";
                    if (!String.IsNullOrEmpty(fileLocation))
                        fileName = fileLocation;
                    else
                    {
                        String pathName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Ripple");
                        if (!System.IO.Directory.Exists(pathName))
                            System.IO.Directory.CreateDirectory(pathName);
                        fileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Ripple", componentName + "LoggingSession" + DateTime.Now.ToString("dd-MM-hh-mm") + ".etl");
                    }
                    sessionName = componentName + "LoggingSession";
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "xperf.exe";
                    startInfo.Arguments = " -start " + sessionName + " -f " + fileName + " -on c0e7ebb5-9430-4a35-8165-3df3fb989c51";
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    Process.Start(startInfo);
                    ComponentCode = componentName;
                    LoggingStarted = true;
                    startInfo = null;
                }
            }
            catch (Exception)
            {}
        }

        public static void StopLogging()
        {
            try
            {
                if (LoggingStarted)
                {
                    //String com = "xperf.exe -stop " + sessionName;
                    //Process.Start(com);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "xperf.exe";
                    startInfo.Arguments = " -stop " + sessionName;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    Process.Start(startInfo);
                    LoggingStarted = false;
                }
            }
            catch (Exception)
            {}
        }

        public static void LogTrace(int level, string formatString, params object[] varargs)
        {
            try
            {
                //Preprocess the format String and send it for logging
                LogTrace_Managed(level, String.Format(ComponentCode + ":" + formatString, varargs));
            }
            catch (Exception)
            {}
        }

        private static void LogTrace_Managed(int level, String message)
        {
            switch (level)
            {
                case 1:
                    ManagedETWEventSource.Logger._MANAGED_1(message);
                    break;
                case 2:
                    ManagedETWEventSource.Logger._MANAGED_2(message);
                    break;
                case 3:
                    ManagedETWEventSource.Logger._MANAGED_3(message);
                    break;
                case 4:
                    ManagedETWEventSource.Logger._MANAGED_4(message);
                    break;
                case 5:
                    ManagedETWEventSource.Logger._MANAGED_5(message);
                    break;
            }
        }
    }
}
