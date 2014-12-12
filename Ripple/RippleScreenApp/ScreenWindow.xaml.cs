using RippleCommonUtilities;
using RippleDictionary;
using RippleScreenApp.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RippleScreenApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ScreenWindow : Window
    {
        internal static RippleDictionary.Ripple rippleData;
        private static TextBlock tbElement = new TextBlock();
        private static TextBlock fullScreenTbElement = new TextBlock();
        private static Image imgElement = new Image();
        private static Image fullScreenImgElement = new Image();
        private static String currentVideoURI = String.Empty;
        private static RippleDictionary.ContentType currentScreenContent = ContentType.Nothing;
        private static bool loopVideo = false;
        private BackgroundWorker myBackgroundWorker;
        private BackgroundWorker pptWorker;
        Utilities.ScriptingHelper helper;
        public System.Windows.Forms.Integration.WindowsFormsHost host;
        public System.Windows.Forms.WebBrowser browserElement;
        internal static String personName;
        private long prevRow;
        private Tile currentTile = null;
        private bool StartVideoPlayed = false;
        public static String sessionGuid = String.Empty;

        public ScreenWindow()
        {
            try
            {
                InitializeComponent();

                LoadData();

                SetObjectProperties();

                //Start receiving messages
                Utilities.MessageReceiver.StartReceivingMessages(this);

            }
            catch (Exception ex)
            {
                //Exit and do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Screen {0}", ex.Message);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.WindowState = System.Windows.WindowState.Maximized;
                ResetUI();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Window Loaded {0}", ex.Message);
            }
        }

        private void SetObjectProperties()
        {
            //Initialize video properties
            this.IntroVideoControl.Source = new Uri(Helper.GetAssetURI(rippleData.Screen.ScreenContents["IntroVideo"].Content));
            this.IntroVideoControl.ScrubbingEnabled = true;
            //Set image elements properties
            imgElement.Stretch = Stretch.Fill;
            fullScreenImgElement.Stretch = Stretch.Fill;
            //Set text block properties
            tbElement.FontSize = 50;
            tbElement.Margin = new Thickness(120, 120, 120, 0);
            tbElement.TextWrapping = TextWrapping.Wrap;
            fullScreenTbElement.FontSize = 50;
            fullScreenTbElement.Margin = new Thickness(120, 120, 120, 0);
            fullScreenTbElement.TextWrapping = TextWrapping.Wrap;
        }

        /// <summary>
        /// Method that loads the configured data for the Screen, right now the Source is XML
        /// </summary>
        private void LoadData()
        {
            try
            {
                //Loads the local dictionary data from the configuration XML
                rippleData = RippleDictionary.Dictionary.GetRipple(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Load Data for Screen {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Resets the UI to System locked mode.
        /// </summary>
        private void ResetUI()
        {
            try
            {
                Globals.ResetGlobals();
                currentVideoURI = String.Empty;
                currentScreenContent = ContentType.Nothing;
                loopVideo = false;
                StartVideoPlayed = false;

                sessionGuid = String.Empty;

                //Pick up content based on the "LockScreen" ID 
                ProjectContent(rippleData.Screen.ScreenContents["LockScreen"]);

                //Commit the telemetry data
                Utilities.TelemetryWriter.CommitTelemetryAsync();
            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Reset UI for Screen {0}", ex.Message);
            }

        }

        /// <summary>
        /// Code will receive messages from the floor
        /// Invoke appropriate content projection based on the tile ID passed
        /// </summary>
        /// <param name="val"></param>
        public void OnMessageReceived(string val)
        {
            try
            {
                //Check for reset
                if (val.Equals("Reset"))
                {
                    //Update the previous entry
                    Utilities.TelemetryWriter.UpdatePreviousEntry();

                    //Reset the system
                    ResetUI();
                }
                //Check for System start
                //Check for System start
                else if (val.StartsWith("System Start"))
                {
                    //Load the telemetry Data
                    Utilities.TelemetryWriter.RetrieveTelemetryData();

                    //The floor has asked the screen to start the system
                    //Get the User Name
                    Globals.UserName = val.Split(':')[1];

                    //Get the person identity for the session
                    personName = String.IsNullOrEmpty(Globals.UserName) ? Convert.ToString(Guid.NewGuid()) : Globals.UserName;

                    Utilities.TelemetryWriter.AddTelemetryRow(rippleData.Floor.SetupID, personName, "Unlock", val, "Unlock");

                    //Set the system state
                    Globals.currentAppState = RippleSystemStates.UserDetected;

                    //Play the Intro Content
                    ProjectIntroContent(rippleData.Screen.ScreenContents["IntroVideo"]);
                }
                //Check for gestures
                else if (val.StartsWith("Gesture"))
                {
                    OnGestureInput(val.Split(':')[1]);
                }
                //Check for HTMl messages
                else if (val.StartsWith("HTML"))
                {
                    OnHTMLMessagesReceived(val.Split(':')[1]);
                }
                //Check for options- TODO need to figure out
                else if (val.StartsWith("Option"))
                {
                    //Do nothing
                }
                //Check if a content - tile mapping or in general content tag exists
                else
                {
                    if (rippleData.Screen.ScreenContents.ContainsKey(val) && rippleData.Screen.ScreenContents[val].Type != ContentType.Nothing)
                    {
                        //Set the system state
                        Globals.currentAppState = RippleSystemStates.OptionSelected;

                        ProjectContent(rippleData.Screen.ScreenContents[val]);

                        RippleCommonUtilities.LoggingHelper.LogTrace(1, "In Message Received {0} {1}:{2}", Utilities.TelemetryWriter.telemetryData.Tables[0].Rows.Count, Utilities.TelemetryWriter.telemetryData.Tables[0].Rows[Utilities.TelemetryWriter.telemetryData.Tables[0].Rows.Count - 1].ItemArray[6], DateTime.Now);

                        //Update the end time for the previous
                        Utilities.TelemetryWriter.UpdatePreviousEntry();

                        //Insert the new entry
                        Utilities.TelemetryWriter.AddTelemetryRow(rippleData.Floor.SetupID, personName, ((currentTile = GetFloorTileForID(val))==null)?"Unknown":currentTile.Name, val, (val == "Tile0") ? "Start" : "Option");
                    }
                    else
                    {
                        //Stop any existing projections
                        DocumentPresentation.HelperMethods.StopPresentation();
                        FullScreenContentGrid.Children.Clear();
                        ContentGrid.Children.Clear();

                        //Set focus for screen window also
                        Utilities.Helper.ClickOnScreenToGetFocus();

                        //Stop any existing videos
                        loopVideo = false;
                        VideoControl.Source = null;
                        FullScreenVideoControl.Source = null;

                        //Clean the images
                        fullScreenImgElement.Source = null;
                        imgElement.Source = null;

                        //Clear the header text
                        TitleLabel.Text = "";

                        //Dispose the objects
                        if(browserElement != null)
                            browserElement.Dispose();
                        browserElement = null;

                        if (host != null)
                            host.Dispose();
                        host = null;

                        if (helper != null)
                            helper.PropertyChanged -= helper_PropertyChanged;
                        helper = null;

                        currentScreenContent = ContentType.Nothing;

                        ShowText("No content available for this option, Please try some other tile option", "No Content");
                    }
                }
            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in On message received for Screen {0}", ex.Message);
            }


        }

        private void OnHTMLMessagesReceived(string p)
        {
            try
            {
                if(p.StartsWith("SessionID,"))
                {
                    sessionGuid = p;
                    return;
                }

                if (helper != null && currentScreenContent == ContentType.HTML)
                {
                    helper.MessageReceived(p);
                }

            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in OnHTMLMessagesReceived received for Screen {0}", ex.Message);
            }
        }

        private void OnGestureInput(string inputGesture)
        {
            try
            {
                //PPT Mode - left and right swipe
                if (inputGesture == GestureTypes.LeftSwipe.ToString() && currentScreenContent == ContentType.PPT)
                {
                    //Acts as previous
                    DocumentPresentation.HelperMethods.GotoPrevious();
                }
                else if (inputGesture == GestureTypes.RightSwipe.ToString() && currentScreenContent == ContentType.PPT)
                {
                    //Check again, Means the presentation ended on clicking next
                    if (!DocumentPresentation.HelperMethods.HasPresentationStarted())
                    {
                        //Change the screen
                        //ShowText("Your presentation has ended, Select some other option", "Select some other option");
                        ShowImage(@"\Assets\Images\pptend.png", "Presentation Ended");

                        //Set focus for screen window also
                        Utilities.Helper.ClickOnScreenToGetFocus();
                    }

                    //Acts as next
                    DocumentPresentation.HelperMethods.GotoNext();

                    //Check again, Means the presentation ended on clicking next
                    if (!DocumentPresentation.HelperMethods.HasPresentationStarted())
                    {
                        //Change the screen text
                        //ShowText("Your presentation has ended, Select some other option", "Select some other option");
                        ShowImage(@"\Assets\Images\pptend.png", "Presentation Ended");

                        //Set focus for screen window also
                        Utilities.Helper.ClickOnScreenToGetFocus();
                    }
                }
                //Browser mode
                else if (currentScreenContent == ContentType.HTML)
                {
                    OnHTMLMessagesReceived(inputGesture.ToString());
                }

                //Set focus for screen window also
                //Utilities.Helper.ClickOnScreenToGetFocus();
            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in OnGestureInput received for Screen {0}", ex.Message);
            }
        }

        #region Content Projection methods
        /// <summary>
        /// Identifies the content type and project accordingly
        /// </summary>
        /// <param name="screenContent"></param>
        private void ProjectContent(RippleDictionary.ScreenContent screenContent)
        {
            try
            {
                if (screenContent.Type == ContentType.HTMLMessage)
                {
                    if (helper != null && currentScreenContent == ContentType.HTML)
                    {
                        helper.MessageReceived(screenContent.Content);
                        return;
                    }
                }

                //Stop any existing projections
                DocumentPresentation.HelperMethods.StopPresentation();
                FullScreenContentGrid.Children.Clear();
                ContentGrid.Children.Clear();

                //Set focus for screen window also
                Utilities.Helper.ClickOnScreenToGetFocus();

                //Stop any existing videos
                loopVideo = false;
                VideoControl.Source = null;
                FullScreenVideoControl.Source = null;

                //Clean the images
                fullScreenImgElement.Source = null;
                imgElement.Source = null;

                //Clear the header text
                TitleLabel.Text = "";

                //Dispose the objects
                if (browserElement != null)
                    browserElement.Dispose();
                browserElement = null;

                if (host != null)
                    host.Dispose();
                host = null;

                if (helper != null)
                    helper.PropertyChanged -= helper_PropertyChanged;
                helper = null;

                currentScreenContent = screenContent.Type;

                if (screenContent.Id == "Tile0" && StartVideoPlayed)
                {
                    currentScreenContent = ContentType.Image;
                    ShowImage("\\Assets\\Images\\default_start.png", screenContent.Header);
                    return;
                }

                switch (screenContent.Type)
                {
                    case RippleDictionary.ContentType.HTML:
                        ShowBrowser(screenContent.Content, screenContent.Header);
                        break;
                    case RippleDictionary.ContentType.Image:
                        ShowImage(screenContent.Content, screenContent.Header);
                        break;
                    case RippleDictionary.ContentType.PPT:
                        ShowPPT(screenContent.Content, screenContent.Header);
                        break;
                    case RippleDictionary.ContentType.Text:
                        ShowText(screenContent.Content, screenContent.Header);
                        break;
                    case RippleDictionary.ContentType.Video:
                        loopVideo = (screenContent.LoopVideo == null) ? false : Convert.ToBoolean(screenContent.LoopVideo);
                        if (screenContent.Id == "Tile0")
                            StartVideoPlayed = true;
                        ShowVideo(screenContent.Content, screenContent.Header);
                        break;
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ProjectContent Method for screen {0}", ex.Message);
            }
        }

        private void ProjectIntroContent(RippleDictionary.ScreenContent screenContent)
        {
            try
            {
                //Dispose the previous content
                //Stop any existing projections
                DocumentPresentation.HelperMethods.StopPresentation();
                FullScreenContentGrid.Children.Clear();
                ContentGrid.Children.Clear();

                //Set focus for screen window also
                Utilities.Helper.ClickOnScreenToGetFocus();

                //Stop any existing videos
                loopVideo = false;
                VideoControl.Source = null;
                FullScreenVideoControl.Source = null;

                //Clean the images
                fullScreenImgElement.Source = null;
                imgElement.Source = null;

                //Clear the header text
                TitleLabel.Text = "";

                if (browserElement != null)
                    browserElement.Dispose();
                browserElement = null;
                if (host != null)
                    host.Dispose();
                host = null;
                if (helper != null)
                    helper.PropertyChanged -= helper_PropertyChanged;
                helper = null;

                //Play the Intro video 
                this.TitleLabel.Text = "";
                ContentGrid.Visibility = Visibility.Collapsed;
                FullScreenContentGrid.Visibility = Visibility.Collapsed;
                FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                IntroVideoControl.Visibility = Visibility.Visible;
                VideoControl.Visibility = Visibility.Collapsed;
                VideoGrid.Visibility = Visibility.Visible;
                IntroVideoControl.Play();
                this.UpdateLayout();

                myBackgroundWorker = new BackgroundWorker();
                myBackgroundWorker.DoWork += myBackgroundWorker_DoWork;
                myBackgroundWorker.RunWorkerCompleted += myBackgroundWorker_RunWorkerCompleted;
                myBackgroundWorker.RunWorkerAsync(rippleData.Floor.Start.IntroVideoWaitPeriod);
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ProjectIntroContent Method for screen {0}", ex.Message);
            }
        }

        private void myBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //System has been started, it just finished playing the intro video
            if (Globals.currentAppState == RippleSystemStates.UserDetected)
            {
                this.IntroVideoControl.Stop();
                //this.IntroVideoControl.Source = null;
                this.IntroVideoControl.Visibility = System.Windows.Visibility.Collapsed;
                ShowGotoStartContent();
            }
        }

        private void myBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(Convert.ToInt16(e.Argument) * 1000);
        }

        /// <summary>
        /// Code to project a video
        /// If the Header value is not provided, the content is projected in full screen mode
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="header"></param>
        private void ShowVideo(String Content, String header)
        {
            try
            {
                //Check if header is null
                //Null - Show full screen content
                if (String.IsNullOrEmpty(header))
                {
                    //Show the full screen video control 
                    currentVideoURI = Helper.GetAssetURI(Content);
                    FullScreenVideoControl.Source = new Uri(currentVideoURI);
                    FullScreenContentGrid.Visibility = Visibility.Collapsed;
                    FullScreenVideoGrid.Visibility = Visibility.Visible;
                    FullScreenVideoControl.Play();
                }
                else
                {
                    TitleLabel.Text = header;
                    currentVideoURI = Helper.GetAssetURI(Content);
                    VideoControl.Source = new Uri(currentVideoURI);
                    ContentGrid.Visibility = Visibility.Collapsed;
                    FullScreenContentGrid.Visibility = Visibility.Collapsed;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                    VideoControl.Visibility = Visibility.Visible;
                    VideoGrid.Visibility = Visibility.Visible;
                    VideoControl.Play();
                }
                this.UpdateLayout();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Show Video method {0}", ex.Message);
            }

        }

        /// <summary>
        /// Code to display text
        /// If the Header value is not provided, the content is projected in full screen mode
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="header"></param>
        private void ShowText(String Content, String header)
        {
            try
            {
                //Check if header is null
                //Null - Show full screen content
                if (String.IsNullOrEmpty(header))
                {
                    //Show the full screen control with text  
                    fullScreenTbElement.Text = Content;
                    FullScreenContentGrid.Children.Clear();
                    FullScreenContentGrid.Children.Add(fullScreenTbElement);
                    FullScreenContentGrid.Visibility = Visibility.Visible;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TitleLabel.Text = header;
                    tbElement.Text = Content;
                    ContentGrid.Children.Clear();
                    ContentGrid.Children.Add(tbElement);
                    ContentGrid.Visibility = Visibility.Visible;
                    FullScreenContentGrid.Visibility = Visibility.Collapsed;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                    VideoGrid.Visibility = Visibility.Collapsed;
                }
                this.UpdateLayout();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Show Text method {0}", ex.Message);
            }
        }

        /// <summary>
        /// Code to project a PPT
        /// If the Header value is not provided, the content is projected in full screen mode
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="header"></param>
        private void ShowPPT(String Content, String header)
        {
            try
            {
                //Check if header is null
                //Null - Show full screen content
                if (String.IsNullOrEmpty(header))
                {
                    //Show the full screen video control 
                    FullScreenContentGrid.Visibility = Visibility.Visible;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TitleLabel.Text = header;
                    ContentGrid.Visibility = Visibility.Visible;
                    FullScreenContentGrid.Visibility = Visibility.Collapsed;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                    VideoGrid.Visibility = Visibility.Collapsed;
                }
                this.UpdateLayout();
                DocumentPresentation.HelperMethods.StartPresentation(Helper.GetAssetURI(Content));

                //ShowText("Please wait while we load your presentation", header);
                //ShowImage(@"\Assets\Images\loading.png", header);
                //this.UpdateLayout();
                //pptWorker = new BackgroundWorker();
                //pptWorker.DoWork += pptWorker_DoWork;
                //pptWorker.RunWorkerAsync(Content);
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Show PPT method {0}", ex.Message);
            }
        }

        void pptWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DocumentPresentation.HelperMethods.StartPresentation(Helper.GetAssetURI(e.Argument.ToString()));
        }

        /// <summary>
        /// Code to project an image
        /// If the Header value is not provided, the content is projected in full screen mode
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="header"></param>
        private void ShowImage(String Content, String header)
        {
            try
            {
                //Check if header is null
                //Null - Show full screen content
                if (String.IsNullOrEmpty(header))
                {
                    //Show the full screen control with text  
                    fullScreenImgElement.Source = new BitmapImage(new Uri(Helper.GetAssetURI(Content)));
                    FullScreenContentGrid.Children.Clear();
                    FullScreenContentGrid.Children.Add(fullScreenImgElement);
                    FullScreenContentGrid.Visibility = Visibility.Visible;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TitleLabel.Text = header;
                    imgElement.Source = new BitmapImage(new Uri(Helper.GetAssetURI(Content)));
                    ContentGrid.Children.Clear();
                    ContentGrid.Children.Add(imgElement);
                    ContentGrid.Visibility = Visibility.Visible;
                    FullScreenContentGrid.Visibility = Visibility.Collapsed;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                    VideoGrid.Visibility = Visibility.Collapsed;
                }
                this.UpdateLayout();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Show Image {0}", ex.Message);
            }
        }

        /// <summary>
        /// Code to show browser based content, applicable for URL's
        /// If the Header value is not provided, the content is projected in full screen mode
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="header"></param>
        private void ShowBrowser(String Content, String header)
        {
            try
            {
                //Check if header is null
                //Null - Show full screen content
                if (String.IsNullOrEmpty(header))
                {
                    //Show the full screen video control  
                    //Display HTML content
                    host = new System.Windows.Forms.Integration.WindowsFormsHost();
                    browserElement = new System.Windows.Forms.WebBrowser();
                    browserElement.ScriptErrorsSuppressed = true;
                    helper = new Utilities.ScriptingHelper(this);
                    browserElement.ObjectForScripting = helper;
                    host.Child = browserElement;
                    helper.PropertyChanged += helper_PropertyChanged;
                    FullScreenContentGrid.Children.Clear();
                    FullScreenContentGrid.Children.Add(host);
                    FullScreenContentGrid.Visibility = Visibility.Visible;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TitleLabel.Text = header;
                    host = new System.Windows.Forms.Integration.WindowsFormsHost();
                    browserElement = new System.Windows.Forms.WebBrowser();
                    browserElement.ScriptErrorsSuppressed = true;
                    helper = new Utilities.ScriptingHelper(this);
                    host.Child = browserElement;
                    browserElement.ObjectForScripting = helper;
                    helper.PropertyChanged += helper_PropertyChanged;
                    ContentGrid.Children.Clear();
                    ContentGrid.Children.Add(host);
                    ContentGrid.Visibility = Visibility.Visible;
                    FullScreenContentGrid.Visibility = Visibility.Collapsed;
                    FullScreenVideoGrid.Visibility = Visibility.Collapsed;
                    VideoGrid.Visibility = Visibility.Collapsed;
                }
                String fileLocation = Helper.GetAssetURI(Content);
                String pageUri = String.Empty;
                //Local file
                if (File.Exists(fileLocation))
                {
                    String[] PathParts = fileLocation.Split(new char[] { ':' });
                    pageUri = "file://127.0.0.1/" + PathParts[0] + "$" + PathParts[1];
                }
                //Web hosted file
                else
                {
                    pageUri = Content;
                }
                browserElement.Navigate(pageUri);
                this.UpdateLayout();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Show Browser {0}", ex.Message);
            }
        }
        #endregion

        #region Helpers
        private Tile GetFloorTileForID(string TileID)
        {
            Tile reqdTile = null;
            try
            {
                reqdTile = rippleData.Floor.Tiles[TileID];
            }
            catch (Exception)
            {
                try
                {
                    reqdTile = rippleData.Floor.Tiles[TileID.Substring(0, TileID.LastIndexOf("SubTile"))].SubTiles[TileID];
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return reqdTile;
        }
        #endregion

        private void VideoControl_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (currentScreenContent == ContentType.Video && loopVideo && (!String.IsNullOrEmpty(currentVideoURI)))
            {
                //Replay the video
                VideoControl.Source = new Uri(currentVideoURI);
                VideoControl.Play();
            }
        }

        private void FullScreenVideoControl_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (currentScreenContent == ContentType.Video && loopVideo && (!String.IsNullOrEmpty(currentVideoURI)))
            {
                //Replay the video
                FullScreenVideoControl.Source = new Uri(currentVideoURI);
                FullScreenVideoControl.Play();
            }
        }

        private void ShowGotoStartContent()
        {
            //Set the system state
            Globals.currentAppState = RippleSystemStates.UserWaitToGoOnStart;
            ProjectContent(rippleData.Screen.ScreenContents["GotoStart"]);
        }

        private void IntroVideoControl_MediaEnded(object sender, RoutedEventArgs e)
        {
            //System has been started, it just finished playing the intro video
            //if (Globals.currentAppState == RippleSystemStates.UserDetected)
            //{
            //    this.IntroVideoControl.Stop();
            //    ShowGotoStartContent();
            //}
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            RippleCommonUtilities.LoggingHelper.StopLogging();
        }

        //Handles messages sent by HTML animations
        void helper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var scriptingHelper = sender as Utilities.ScriptingHelper;
                if (scriptingHelper != null)
                {
                    if (e.PropertyName == "SendMessage")
                    {
                        if ((!String.IsNullOrEmpty(scriptingHelper.SendMessage)) && currentScreenContent == ContentType.HTML)
                        {
                            //Send the screen a message for HTML parameter passing
                            Utilities.MessageReceiver.SendMessage("HTML:" + scriptingHelper.SendMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in helper property changed event {0}", ex.Message);
            }

        }

    }
}
