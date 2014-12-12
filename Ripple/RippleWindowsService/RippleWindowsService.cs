using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RippleWindowsService
{
    public partial class RippleWindowsService : ServiceBase
    {
        private System.Timers.Timer iRippleWindowsServiceTimer = null;
        private String sTimeToSend = null;

        private static String TelemetryFilePath
        {
            get { return Path.Combine(Path.GetTempPath(), "Ripple", "RippleTelemetryData.xml"); }
        }

        private static String TargetTableName = System.Configuration.ConfigurationManager.AppSettings["TargetTableName"];
        private static String TargetDatabaseName = System.Configuration.ConfigurationManager.AppSettings["TargetDatabaseName"];
        private static String TargetServerName = System.Configuration.ConfigurationManager.AppSettings["TargetServerName"];


        public RippleWindowsService()
        {
            InitializeComponent();

            //Initialize event logging
            if (!System.Diagnostics.EventLog.SourceExists("RippleSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "RippleSource", "RippleServiceLogs");
            }
            RippleLogEvent.Source = "RippleSource";
            RippleLogEvent.Log = "RippleServiceLogs";

           
        }

        protected override void OnStart(string[] args)
        {
            sTimeToSend = System.Configuration.ConfigurationManager.AppSettings["Hour"];

            //Initialize Timer
            iRippleWindowsServiceTimer = new System.Timers.Timer(3600000);
            iRippleWindowsServiceTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.RippleWindowsServiceTimer_Tick);
            iRippleWindowsServiceTimer.Enabled = true;

            RippleLogEvent.WriteEntry("Ripple Windows Service Started");
        }

        protected override void OnStop()
        {
            RippleLogEvent.WriteEntry("Ripple Windows Service Stopped");
        }

        private void RippleWindowsServiceTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            iRippleWindowsServiceTimer.Enabled = false;

            string sCurrentHour = DateTime.Now.Hour.ToString() + ":00";
            DateTime dCurrentHour = Convert.ToDateTime(sCurrentHour);
            DateTime dTimeToSend = Convert.ToDateTime(sTimeToSend);
            int iComparison = DateTime.Compare(dTimeToSend, dCurrentHour);
            if (iComparison == 0)
            {
                //Update the xml to database
                 UpdateTelemetry();

                //Email the etl files to Ripple Team

            }
            iRippleWindowsServiceTimer.Enabled = true;
        }

        private void UpdateTelemetry()
        {
            DataSet telemetryData = new DataSet();
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

                    //Insert in the Database
                    using (SqlConnection sqlConn = new SqlConnection(GetConnectionString()))
                    {
                        sqlConn.Open();
                        //Insert the new data
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn))
                        {
                            bulkCopy.DestinationTableName = TargetTableName;

                            bulkCopy.WriteToServer(telemetryData.Tables[0]);
                        }
                        sqlConn.Close();

                        //Successfull, hence delete the file
                        File.Delete(TelemetryFilePath);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                reader = null;
                if (telemetryFile != null)
                {
                    telemetryFile.Close();
                    telemetryFile.Dispose();
                }
                RippleLogEvent.WriteEntry(String.Format("Went wrong in uploading the telemetry data to teh database {0}", ex.Message));
            }
        }

        private String GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.InitialCatalog = TargetDatabaseName;
            builder.DataSource = TargetServerName;
            builder.IntegratedSecurity = true;
            builder.ConnectTimeout = 12;

            return builder.ConnectionString;
        }
    }
}
