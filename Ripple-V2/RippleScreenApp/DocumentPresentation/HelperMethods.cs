using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Speech.Synthesis;
using RippleCommonUtilities;


namespace RippleScreenApp.DocumentPresentation
{
    public static class HelperMethods
    {
        #region properties

        /// <summary>
        /// Stores the type of document to be projected
        /// </summary>
        private static DocType g_doctype;

        /// <summary>
        /// Stores the folder location from where to pick the files
        /// </summary>
        private static String g_FolderLocation;

        private static IMasterDocumentClass g_documentClass;

        static BackgroundWorker myBackgroundWorker;
       
        #endregion

        /// <summary>
        /// To check if any of the document presentation has started
        /// </summary>
        /// <returns></returns>
        public static bool HasPresentationStarted()
        {
            if (g_documentClass == null)
                return false;
            return g_documentClass.ApplicationStarted();
        }

        /// <summary>
        /// To start the presentation
        /// </summary>
        /// <param name="fileName"></param>
        public static void StartPresentation(String fileName)
        {
            try
            {
                if (!DocumentPresentation.HelperMethods.HasPresentationStarted())
                {
                    //Set focus for the Floor window
                    RippleCommonUtilities.HelperMethods.ClickOnFloorToGetFocus();
                    //Set focus for screen window also
                    Utilities.Helper.ClickOnScreenToGetFocus();

                    //Find document type
                    g_doctype = GetDocumentType(fileName);
                    switch (g_doctype)
                    {
                        case DocType.PPT:
                            g_documentClass = new PPTDocumentClass(fileName);
                            break;
                    }
                    if (g_documentClass == null)
                        return;
                    g_documentClass.StartPresentation();
                    g_documentClass.SetApplicationStatus(true);

                    //Speak out
                    myBackgroundWorker = new BackgroundWorker();
                    myBackgroundWorker.DoWork += myBackgroundWorkerForSpeech_DoWork;
                    myBackgroundWorker.RunWorkerAsync();

                    //Set focus for screen window also
                    //Utilities.Helper.ClickOnScreenToGetFocus();

                }
            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in StartPresentation for Screen {0}", ex.Message);
            }

        }

        private static void myBackgroundWorkerForSpeech_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SpeechSynthesizer synClass = new SpeechSynthesizer())
            {
                synClass.SetOutputToDefaultAudioDevice();
                synClass.Volume = 100;
                synClass.Speak("To explore more, swipe your right hand. To go back, swipe your left hand");
            }
        }
        
        /// <summary>
        /// On Stop
        /// </summary>
        public static void StopPresentation()
        {
            try
            {
                if (HelperMethods.HasPresentationStarted())
                {
                    if (g_documentClass == null)
                        return;
                    g_documentClass.StopPresentation();
                    g_documentClass.SetApplicationStatus(false);
                    //Set focus for the Floor window
                    RippleCommonUtilities.HelperMethods.ClickOnFloorToGetFocus();
                    //Set focus for screen window also
                    Utilities.Helper.ClickOnScreenToGetFocus();

                }
            }
            catch (Exception ex)
            {
               //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Stop Presentation for Screen {0}", ex.Message);
            }

        }

        /// <summary>
        /// On Next
        /// </summary>
        public static void GotoNext()
        {
            try
            {
                if (HelperMethods.HasPresentationStarted())
                {
                    if (g_documentClass == null)
                        return;
                    g_documentClass.GotoNext();

                    //Check again
                    //if (!HelperMethods.HasPresentationStarted())
                    //{
                    //    //Set focus for screen window also
                    //    Utilities.Helper.ClickOnScreenToGetFocus();
                    //}
                }
            }
            catch (Exception ex)
            {
               //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Goto Next for Screen {0}", ex.Message);
            }

        }

        /// <summary>
        /// On Previous
        /// </summary>
        public static void GotoPrevious()
        {
            try
            {
                if (HelperMethods.HasPresentationStarted())
                {
                    if (g_documentClass == null)
                        return;
                    g_documentClass.GotoPrevious();
                }
            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in GotoPrevious for Screen {0}", ex.Message);
            }

        }

        /// <summary>
        /// Gets the files present in the folder indicated by g_FolderLocation and interacts with the user to get the filename to be projected.
        /// </summary>
        private static DocType GetDocumentType(String fileName)
        {
            String fileExtension = fileName.Substring(fileName.IndexOf(".") + 1);
            if (fileExtension.ToLower().Contains("ppt"))
                return DocType.PPT;
            return DocType.NOT_DEFINED;
        }       
    }
}