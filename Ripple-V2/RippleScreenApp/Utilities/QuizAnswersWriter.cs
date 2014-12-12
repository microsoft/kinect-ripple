using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RippleScreenApp.Utilities
{
    public static class QuizAnswersWriter
    {
        private static String QuizAnswersFilePath
        {
            get { return Path.Combine(Path.GetTempPath(), "Ripple", "RippleQuizAnswersData.xml"); }
        }

        private delegate void CommitQuizAnswersDelegate(String quizAnswersData, String personName, String setupID);
        public static void CommitQuizAnswersAsync(String quizAnswersData, String personName, String setupID)
        {
            CommitQuizAnswersDelegate asyncDelegate = new CommitQuizAnswersDelegate(CommitQuizAnswers);
            AsyncOperation operation = AsyncOperationManager.CreateOperation(null);
            asyncDelegate.BeginInvoke(quizAnswersData, personName, setupID, null, operation);
        }

        private static void CommitQuizAnswers(String quizAnswers, String personName, String setupID)
        {
            XmlSerializer writer;
            StreamWriter quizAnswersFile = null;
            DataSet quizData = null;
            try
            {
                if (String.IsNullOrEmpty(quizAnswers))
                    return;

                quizData = RetrieveQuizAnswersData();

                //Create the dataset of answers
                quizData.Tables[0].Rows.Add(setupID, personName, quizAnswers, DateTime.Now);

                writer = new XmlSerializer(typeof(DataSet));
                quizAnswersFile = new StreamWriter(QuizAnswersFilePath);
                writer.Serialize(quizAnswersFile, quizData);
                quizAnswersFile.Close();
                quizAnswersFile.Dispose();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in CommitQuizAnswers at Screen side {0}", ex.Message);
                writer = null;
                if (quizAnswersFile != null)
                {
                    quizAnswersFile.Close();
                    quizAnswersFile.Dispose();
                }
            }
        }

        private static DataSet RetrieveQuizAnswersData()
        {
            XmlSerializer reader;
            StreamReader quizAnswersFile = null;
            DataSet quizAnswersData = new DataSet();
            try
            {
                if (File.Exists(QuizAnswersFilePath))
                {
                    reader = new XmlSerializer(typeof(DataSet));
                    quizAnswersFile = new StreamReader(QuizAnswersFilePath);
                    quizAnswersData = (DataSet)reader.Deserialize(quizAnswersFile);
                    quizAnswersFile.Close();
                    quizAnswersFile.Dispose();
                    return quizAnswersData;
                }
                else
                {
                    //Initialize the Dataset and return it
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("SetupID", typeof(String)));
                    dt.Columns.Add(new DataColumn("PersonName", typeof(String)));
                    dt.Columns.Add(new DataColumn("Answers", typeof(String)));
                    dt.Columns.Add(new DataColumn("TimeStamp", typeof(String)));
                    quizAnswersData.Tables.Add(dt);
                    return quizAnswersData;
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in RetrieveQuizAnswersData at Screen side {0}", ex.Message);
                reader = null;
                if (quizAnswersFile != null)
                {
                    quizAnswersFile.Close();
                    quizAnswersFile.Dispose();
                }
                return null;
            }
        }
    }
}
