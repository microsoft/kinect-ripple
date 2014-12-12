using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Diagnostics;
using System.Windows;

namespace RippleScreenApp.DocumentPresentation
{
    internal interface IMasterDocumentClass
    {
        bool ApplicationStarted();
        void StartPresentation();
        void StopPresentation();
        void GotoNext();
        void GotoPrevious();
        void KillApplications();
        void SetApplicationStatus(bool status);
    }

    internal class PPTDocumentClass : IMasterDocumentClass
    {
        private static PowerPoint.Application g_PPT_Application;
        private static PowerPoint.Presentations g_PPT_CurrentPresentations;
        private static PowerPoint.Presentation g_PPT_PresentationToBeProjected;
        private static String g_PPT_FileLocation;
        private static bool g_ApplicationStarted = false;

        public PPTDocumentClass(String filePath)
        {
            g_PPT_FileLocation = filePath;
        }

        private void LaunchApplication()
        {
            if (g_PPT_Application == null)
                g_PPT_Application = new PowerPoint.Application();

            //g_PPT_Application.Visible = MsoTriState.msoTrue;
            //g_PPT_Application.SlideShowBegin += g_PPT_Application_SlideShowBegin;
        }

        //void g_PPT_Application_SlideShowBegin(PowerPoint.SlideShowWindow Wn)
        //{
        //    //TODO
        //    Wn.View.State = PowerPoint.PpSlideShowState.ppSlideShowRunning;
        //    Wn.Activate();
        //}

        public bool ApplicationStarted()
        {
            return g_ApplicationStarted;
        }

        public void SetApplicationStatus(bool i_Status)
        {
            g_ApplicationStarted = i_Status;
        }

        public void KillApplications()
        {
            //Kill All the processes
            var pptProcesses = Process.GetProcessesByName("powerpnt");
            foreach (var p in pptProcesses)
                p.Kill();
            g_ApplicationStarted = false;
            g_PPT_Application = null;
            g_PPT_PresentationToBeProjected = null;
        }

        public void StartPresentation()
        {
            try
            {
                LaunchApplication();
                g_PPT_CurrentPresentations = g_PPT_Application.Presentations;
                g_PPT_PresentationToBeProjected = g_PPT_CurrentPresentations.Open(g_PPT_FileLocation, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
                //g_PPT_PresentationToBeProjected.SlideShowSettings.ShowPresenterView = MsoTriState.msoFalse;
                //g_PPT_PresentationToBeProjected.SlideShowSettings.StartingSlide = 1;
                //g_PPT_PresentationToBeProjected.SlideShowSettings.ShowType = PowerPoint.PpSlideShowType.ppShowTypeSpeaker;
                //g_PPT_PresentationToBeProjected.SlideShowSettings.AdvanceMode = PowerPoint.PpSlideShowAdvanceMode.ppSlideShowUseSlideTimings;
                //g_PPT_PresentationToBeProjected.SlideShowSettings.ShowWithAnimation = MsoTriState.msoTrue;
                g_PPT_PresentationToBeProjected.SlideShowSettings.Run();
                g_PPT_Application.PresentationClose += g_PPT_Application_PresentationClose;
                //g_PPT_Application.Activate();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                //Do nothing
            }
        }

        private void ResetClassVariables()
        {
            g_PPT_Application = null;
            g_PPT_PresentationToBeProjected = null;
            g_PPT_CurrentPresentations = null;
            g_PPT_FileLocation = String.Empty;
        }

        private void g_PPT_Application_PresentationClose(PowerPoint.Presentation Pres)
        {
            ResetClassVariables();
            SetApplicationStatus(false);
        }

        public void StopPresentation()
        {
            try
            {
                if (g_PPT_Application != null && g_PPT_PresentationToBeProjected != null && g_PPT_Application.SlideShowWindows.Count > 0)
                {
                    g_PPT_PresentationToBeProjected.Close();
                    g_PPT_Application.Quit();
                    ResetClassVariables();
                }
            }
            catch (Exception)
            {

            }
        }

        public void GotoNext()
        {
            try
            {
                if (g_PPT_Application != null)
                {
                    ((PowerPoint.SlideShowWindow)g_PPT_Application.SlideShowWindows._Index(1)).Activate();
                    PowerPoint.SlideShowWindow sd = ((PowerPoint.SlideShowWindow)g_PPT_Application.SlideShowWindows._Index(1));
                    int currentVal = sd.View.CurrentShowPosition;
                    int totalSlides = g_PPT_PresentationToBeProjected.Slides.Count;
                    if (currentVal >= totalSlides)
                    {
                        KillApplications();
                        //Click on the floor to get focus
                        RippleCommonUtilities.HelperMethods.ClickOnFloorToGetFocus();
                        //Set focus for screen window also
                        Utilities.Helper.ClickOnScreenToGetFocus();
                    }
                    else
                    {
                        ((PowerPoint.SlideShowWindow)g_PPT_Application.SlideShowWindows._Index(1)).View.Next();
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                //Stop the application if the SlideShowWindow has closed.
                //if(ex.Message.Contains("Integer out of range. 1 is not in the valid range of 1 to 0"))
                //Stop either ways
                KillApplications();
                ResetClassVariables();
                //Click on the floor to get focus
                RippleCommonUtilities.HelperMethods.ClickOnFloorToGetFocus();
                //Set focus for screen window also
                Utilities.Helper.ClickOnScreenToGetFocus();
            }
            catch (Exception)
            {
            }
        }

        public void GotoPrevious()
        {
            try
            {
                if (g_PPT_Application != null)
                {
                    ((PowerPoint.SlideShowWindow)g_PPT_Application.SlideShowWindows._Index(1)).Activate();
                    ((PowerPoint.SlideShowWindow)g_PPT_Application.SlideShowWindows._Index(1)).View.Previous();
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                //Do nothing
            }
            catch (Exception)
            {
            }
        }
    }
}
