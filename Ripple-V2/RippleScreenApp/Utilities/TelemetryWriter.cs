using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace RippleScreenApp.Utilities
{
    public static class TelemetryWriter
    {
        internal static DataSet telemetryData;

        private static DateTime currentTime;
        private static String setupID;
        private static String personName;
        private static String TileName;
        private static String TileID;
        private static String Option;
        private static DateTime oldTime;

        private static String TelemetryFilePath
        {
            get { return Path.Combine(Path.GetTempPath(), "Ripple", "RippleTelemetryData.xml"); }
        }

        private delegate void CommitTelemetryDelegate();
        public static void CommitTelemetryAsync()
        {
            CommitTelemetryDelegate asyncDelegate = new CommitTelemetryDelegate(CommitTelemetry);
            AsyncOperation operation = AsyncOperationManager.CreateOperation(null);
            asyncDelegate.BeginInvoke(null, operation);
        }
       
        public static void CommitTelemetry()
        {
            XmlSerializer writer;
            StreamWriter telemetryFile = null;
            try
            {
                if (telemetryData == null)
                    return;

                writer = new XmlSerializer(typeof(DataSet));
                telemetryFile = new StreamWriter(TelemetryFilePath);
                writer.Serialize(telemetryFile, telemetryData);
                telemetryFile.Close();
                telemetryFile.Dispose();
                telemetryData = null;
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in CommitTelemetry at Screen side {0}", ex.Message);
                writer = null;
                if (telemetryFile != null)
                {
                    telemetryFile.Close();
                    telemetryFile.Dispose();
                }
                telemetryData = null;
            }
        }

        public static void RetrieveTelemetryData()
        {
            XmlSerializer reader;
            StreamReader telemetryFile = null;
            try
            {
                if (File.Exists(TelemetryFilePath))
                {
                    reader = new XmlSerializer(typeof(DataSet));
                    telemetryFile = new StreamReader(TelemetryFilePath);
                    telemetryData = (DataSet)reader.Deserialize(telemetryFile);
                    telemetryFile.Close();
                    telemetryFile.Dispose();
                }
                else
                {
                    telemetryData = new DataSet();
                    //Initialize the Dataset and return it
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("SetupID", typeof(String)));
                    dt.Columns.Add(new DataColumn("PersonID", typeof(String)));
                    dt.Columns.Add(new DataColumn("TileName", typeof(String)));
                    dt.Columns.Add(new DataColumn("TileID", typeof(String)));
                    dt.Columns.Add(new DataColumn("OptionType", typeof(String)));
                    dt.Columns.Add(new DataColumn("StartTime", typeof(DateTime)));
                    dt.Columns.Add(new DataColumn("EndTime", typeof(DateTime)));
                    telemetryData.Tables.Add(dt);
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in RetrieveTelemetryData at Screen side {0}", ex.Message);
                reader = null;
                if (telemetryFile != null)
                {
                    telemetryFile.Close();
                    telemetryFile.Dispose();
                }
            }
        }

        public static void AddTelemetryRow(String setupID, String personName, String TileName, String TileID, String Option)
        {
            if(telemetryData != null)
            {
                currentTime = DateTime.Now;
                telemetryData.Tables[0].Rows.Add(setupID, personName, TileName, TileID, Option, currentTime, currentTime);
            }
        }

        public static void UpdatePreviousEntry()
        {
            if (telemetryData != null && telemetryData.Tables[0].Rows.Count >= 1)
            {
                //Get the previous entry
                setupID = Convert.ToString(telemetryData.Tables[0].Rows[telemetryData.Tables[0].Rows.Count - 1].ItemArray[0]);
                personName = Convert.ToString(telemetryData.Tables[0].Rows[telemetryData.Tables[0].Rows.Count - 1].ItemArray[1]);
                TileName = Convert.ToString(telemetryData.Tables[0].Rows[telemetryData.Tables[0].Rows.Count - 1].ItemArray[2]);
                TileID = Convert.ToString(telemetryData.Tables[0].Rows[telemetryData.Tables[0].Rows.Count - 1].ItemArray[3]);
                Option = Convert.ToString(telemetryData.Tables[0].Rows[telemetryData.Tables[0].Rows.Count - 1].ItemArray[4]);
                oldTime = Convert.ToDateTime(telemetryData.Tables[0].Rows[telemetryData.Tables[0].Rows.Count - 1].ItemArray[5]);
                currentTime = DateTime.Now;
                telemetryData.Tables[0].Rows[telemetryData.Tables[0].Rows.Count - 1].ItemArray = new object[] { setupID, personName, TileName, TileID, Option, oldTime, currentTime};
            }
        }
    }
}
