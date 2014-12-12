using System;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace RippleFloorApp.Utilities
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public class ScriptingHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        private bool systemUnlocked = false;
        public bool SystemUnlocked
        {
            get { return systemUnlocked; }
            set
            {
                systemUnlocked = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SystemUnlocked"));
            }
        }

        private bool exitGame = false;
        public bool ExitGame
        {
            get { return exitGame; }
            set
            {
                exitGame = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ExitGame"));
            }
        }

        private bool exitOnStart = false;
        public bool ExitOnStart
        {
            get { return exitOnStart; }
            set
            {
                exitOnStart = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ExitOnStart"));
            }
        }

        private String sendMessage = String.Empty;
        public String SendMessage
        {
            get { return sendMessage; }
            set
            {
                sendMessage = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SendMessage"));
            }
        }

        FloorWindow mExternalWPF;
        public ScriptingHelper(FloorWindow w)
        {
            this.mExternalWPF = w;
        }
        
        public void MessageReceived(String messageParam)
        {
            try
            {
                this.mExternalWPF.browserElement.Document.InvokeScript("executeCommandFromScreen", new Object[]{messageParam});
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in MessageReceived of scripting Helper for Floor {0}", ex.Message);
            }
        }

        public void GestureReceived(RippleCommonUtilities.GestureTypes ges)
        {
            try
            {
                this.mExternalWPF.browserElement.Document.InvokeScript("gestureReceived", new object[] { ges.ToString()});
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in GestureReceived {0}", ex.Message);
            }
        }

        public void executeCommand(String commandText, String commandParameters)
        {
            //RippleCommonUtilities.LoggingHelper.LogTrace(1, "Command Recieved {0} with Parameters {1}", commandText, commandParameters);
            try
            {
                String[] parameters = commandParameters.Split(new Char[] { ',' });
                bool commandExecuted = false;
                exitGame = false;
                exitOnStart = false;
                switch (commandText)
                {
                    case "unlockSystem":
                        if (parameters[0] == String.Empty && parameters.Length == 1)
                        {
                            SystemUnlocked = true;
                            commandExecuted = true;
                        }
                        break;
                    case "exitGame":
                        //Raise it to the floor
                        ExitGame = true;
                        commandExecuted = true;
                        break;
                    case "exitOnStart":
                        //Raise it to the floor
                        ExitOnStart = true;
                        commandExecuted = true;
                        break;
                    case "playAudio":
                        PlayAudio(commandParameters);
                        commandExecuted = true;
                        break;
                    case "sendCommandToFrontScreen":
                        SendMessage = "HTML:" + commandParameters;
                        commandExecuted = true;
                        break;
                    case "sendCommandForScreenProcessing":
                        SendMessage = commandParameters;
                        commandExecuted = true;
                        break;
                    case "logMessage":
                        RippleCommonUtilities.LoggingHelper.LogTrace(1, "Log Message from HTML {0}", commandParameters);
                        commandExecuted = true;
                        break;
                }
                if (!commandExecuted)
                {
                    RippleCommonUtilities.LoggingHelper.LogTrace(1, "Command {0} with Parameters {1} not Supported", commandText, commandParameters);

                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in executeCommand for Scripting helper {0}", ex.Message);
            }

        }

        private void PlayAudio(string commandParameters)
        {
            using (var bg = new BackgroundWorker())
            {
                bg.DoWork += bg_DoWork;                
                bg.RunWorkerAsync(commandParameters);
            }
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SpeechSynthesizer synClass = new SpeechSynthesizer())
            {
                synClass.SetOutputToDefaultAudioDevice();
                synClass.Volume = 100;
                synClass.Speak(Convert.ToString(e.Argument));
            }
        }
    }

}
