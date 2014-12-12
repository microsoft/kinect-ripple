using RippleCommonUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RippleDictionary;
using System.Media;
using System.Windows.Media.Animation;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using TUC = RippleFloorApp.Controls.TileControl;
using SM = System.Windows.Media;
using System.Reflection;

namespace RippleFloorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FloorWindow : Window
    {
        #region Properties
        public const string InnerContent = "InnerContent";
        public const string InnerTile = "InnerTile";
        public const string Label = "Label";
        private List<TUC> tileList = null;
        private static Random rnd = new Random();

        /// <summary>
        /// Windows forms host to host windows forms controls
        /// </summary>
        WindowsFormsHost host;

        ///// <summary>
        ///// Flash control - type of windows forms' control
        ///// </summary>
        //FlashAxControl player;

        DateTime now;

        public static RippleDictionary.Floor floorData;

        static String lastSelectedOptionName;

        Utilities.ScriptingHelper helper;
        public System.Windows.Forms.Integration.WindowsFormsHost browserHost;
        public System.Windows.Forms.WebBrowser browserElement;

        /// <summary>
        /// Kinect helper - connected to floor and helps detect location and gestures
        /// </summary>
        Utilities.KinectHelper kinectHelper;

        BackgroundWorker waitThread;

        private static BackgroundWorker autoLogoutWorker = new BackgroundWorker();

        private static int autoLockPeriodInSeconds = 0;
        private static int checkPeriodInSeconds = 0;

        static int waitDuration = 0;

        static bool QRMode;

        #region AnimationProperties
        Storyboard tileTransitionSB;
        Storyboard liveTile;
        #endregion
        #endregion

        /// <summary>
        /// Constructor for the class
        /// Used to accomplish the one time activities
        /// </summary>
        public FloorWindow()
        {
            try
            {
                InitializeComponent();

                LoadData();

                InitializeTiles();

                InitializeAnimationStoryboards();

                //Start receiving messages
                Utilities.MessageSender.StartReceivingMessages(this);

                kinectHelper = new Utilities.KinectHelper();

                kinectHelper.PropertyChanged += kinectHelper_PropertyChanged;

                //Initialize background wait thread to auto lock the system
                autoLockPeriodInSeconds = floorData.SystemAutoLockPeriod;
                checkPeriodInSeconds = 60;
                autoLogoutWorker.DoWork += autoLogoutWorker_DoWork;
                autoLogoutWorker.RunWorkerCompleted += autoLogoutWorker_RunWorkerCompleted;

                //Start the auto - lock thread
                autoLogoutWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                //Exit and do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Floor constructor {0}", ex.Message);
            }
        }

        /// <summary>
        /// Code will receive messages from the screen
        /// </summary>
        /// <param name="val"></param>
        public void OnMessageReceived(string val)
        {
            try
            {
                //Check for HTMl messages
                if (val.StartsWith("HTML"))
                {
                    OnHTMLMessagesReceived(val.Split(':')[1]);
                }
            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in On message received for Floor {0}", ex.Message);
            }


        }

        private void OnHTMLMessagesReceived(string p)
        {
            try
            {
                if (helper != null && GetFloorTileForID(Globals.SelectedOptionFullName).Action == TileAction.HTML)
                {
                    helper.MessageReceived(p);
                }

            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in OnHTMLMessagesReceived received for floor {0}", ex.Message);
            }
        }

        #region System Initialize

        private void LoadData()
        {
            try
            {
                //Loads the local dictionary data from the configuration XML
                floorData = RippleDictionary.Dictionary.GetFloor(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Load Data for Floor {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Method to initialize the tile layout dynamically
        /// </summary>
        private void InitializeTiles()
        {
            this.MainContainer.Children.Clear();

            //Populate the list of tiles
            if (tileList == null)
            {
                //Upper tile properties
                Controls.UpperVideoControl UpperVideoContentGrid = new Controls.UpperVideoControl(this);
                UpperVideoContentGrid.ControlWidth = floorData.UpperTile.Style.Width;
                UpperVideoContentGrid.ControlHeight = floorData.UpperTile.Style.Height;
                UpperVideoContentGrid.SetMargin(floorData.UpperTile.Coordinate.X, floorData.UpperTile.Coordinate.Y);

                TUC nTile;
                tileList = new List<TUC>();
                //Rest of the tiles including start
                foreach (var tile in floorData.Tiles.Values)
                {
                    nTile = new TUC();
                    nTile.TileBackground = tile.Color;
                    nTile.TileIDName = nTile.TileIDName.Replace("TileID", tile.Id);
                    nTile.InnerContentTileIDName = nTile.InnerContentTileIDName.Replace("TileID", tile.Id);
                    nTile.TileIDLabelName = nTile.TileIDLabelName.Replace("TileID", tile.Id);
                    nTile.TileWidth = tile.Style.Width;
                    nTile.TileHeight = tile.Style.Height;
                    if (tile.Name == "Start")
                        nTile.TileIDLabel.FontSize = 40;
                    nTile.SetMargin(tile.Coordinate.X, tile.Coordinate.Y);
                    nTile.TileIDLabelText = tile.Name;
                    this.RegisterName(nTile.TileIDName, nTile.TileID);
                    this.RegisterName(nTile.InnerContentTileIDName, nTile.InnerContentTileID);
                    this.RegisterName(nTile.TileIDLabelName, nTile.TileIDLabel);
                    tileList.Add(nTile);
                }

                //Main Option grid properties
                Controls.MainOptionTile mainOptionGrid = new Controls.MainOptionTile(this);
                mainOptionGrid.SetMargin(floorData.UpperTile.Style.Height, 0);
                mainOptionGrid.ControlWidth = floorData.UpperTile.Style.Width;
                mainOptionGrid.ControlHeight = floorData.UpperTile.Style.Height;

                //Overlay image properties
                Controls.OverlayImageControl overlayImage = new Controls.OverlayImageControl(this);
                overlayImage.SetMargin(floorData.UpperTile.Style.Height);

                //Add upper video tile to the main UI
                this.MainContainer.Children.Add(UpperVideoContentGrid);

                //Add the tile list to the main UI
                for (int i = tileList.Count - 1; i >= 0; i--)
                {
                    this.MainContainer.Children.Add(tileList[i]);
                }

                //Add the overlay image and Main Option grid to the Main UI
                this.MainContainer.Children.Add(mainOptionGrid);
                this.MainContainer.Children.Add(overlayImage);

                this.UpdateLayout();
            }
        }

        private void InitializeAnimationStoryboards()
        {
            //create the Animation for tile transition
            CreateTileTransitionStoryboard(floorData.Transition.Animation);

            //Set the keyframes based on the resolution for live tile
            liveTile = (Storyboard)this.Main.FindResource("liveTile");
            double OriginalHorizontalResolutionUsed = 1150;
            double ratio = Globals.CurrentResolution.HorizontalResolution / OriginalHorizontalResolutionUsed;
            double val;
            foreach (EasingDoubleKeyFrame sbItem in ((DoubleAnimationUsingKeyFrames)liveTile.Children[0]).KeyFrames)
            {
                val = sbItem.Value * ratio;
                sbItem.SetValue(EasingDoubleKeyFrame.ValueProperty, val);
                //((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)liveTile.Children[0]).KeyFrames[index]).SetValue(EasingDoubleKeyFrame.ValueProperty, val);
            }
            foreach (EasingDoubleKeyFrame sbItem in ((DoubleAnimationUsingKeyFrames)liveTile.Children[1]).KeyFrames)
            {
                val = sbItem.Value * ratio;
                sbItem.SetValue(EasingDoubleKeyFrame.ValueProperty, val);
                //((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)liveTile.Children[0]).KeyFrames[index++]).SetValue(EasingDoubleKeyFrame.ValueProperty, val);
            }
        }

        private void CreateTileTransitionStoryboard(string tileTransitionName)
        {
            Storyboard existingTileTransition = (Storyboard)this.Main.FindResource(tileTransitionName);

            tileTransitionSB = new Storyboard();

            DoubleAnimationUsingKeyFrames doubleAnim = (DoubleAnimationUsingKeyFrames)existingTileTransition.Children[0];

            ColorAnimationUsingKeyFrames colorAnim = (ColorAnimationUsingKeyFrames)existingTileTransition.Children[1];

            DoubleAnimationUsingKeyFrames dAnim = null;
            ColorAnimationUsingKeyFrames cAnim = null;
            tileTransitionSB.Children.Clear();
            //Add the above two animations for every tile except start
            foreach (var tile in floorData.Tiles.Keys)
            {
                if (!tile.Equals("Tile0"))
                {
                    dAnim = doubleAnim.Clone();
                    cAnim = colorAnim.Clone();
                    Storyboard.SetTarget(dAnim, (Grid)this.FindName(tile));
                    Storyboard.SetTarget(cAnim, (Grid)this.FindName(tile));
                    tileTransitionSB.Children.Add(dAnim);
                    tileTransitionSB.Children.Add(cAnim);
                }
            }
        }

        #region Unlock code

        //Handles messages sent by HTML animations
        void helper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var scriptingHelper = sender as Utilities.ScriptingHelper;
                if (scriptingHelper != null)
                {
                    if (e.PropertyName == "SystemUnlocked")
                    {
                        //Check if the system is configured to be unlocked through WPF animations, and its been unlocked
                        if (scriptingHelper.SystemUnlocked && floorData.Start.Unlock.Mode == Mode.HTML)
                        {
                            if (helper != null)
                            {
                                helper.PropertyChanged -= helper_PropertyChanged;
                                helper = null;
                            }

                            UnlockRippleSystem();
                        }
                    }

                    else if (e.PropertyName == "ExitOnStart")
                    {
                        if (scriptingHelper.ExitOnStart && Globals.currentAppState == RippleSystemStates.UserPlayingAnimations)
                        {
                            //End the game and go to start
                            if (helper != null)
                            {
                                helper.PropertyChanged -= helper_PropertyChanged;

                                if (browserElement != null)
                                    browserElement.Dispose();
                                browserElement = null;

                                if (browserHost != null)
                                    browserHost.Dispose();
                                browserHost = null;

                                helper = null;
                            }
                            //Start the video
                            ((MediaElement)this.FindName("FloorVideoControl")).Play();

                            lastSelectedOptionName = "Tile0";

                            //Show the main options
                            OnStartSelected();
                        }
                    }

                    else if (e.PropertyName == "ExitGame")
                    {
                        if (scriptingHelper.ExitGame && Globals.currentAppState == RippleSystemStates.UserPlayingAnimations)
                        {
                            //End the game and go to start with the main options laid out
                            if (helper != null)
                            {
                                helper.PropertyChanged -= helper_PropertyChanged;

                                if (browserElement != null)
                                    browserElement.Dispose();
                                browserElement = null;

                                if (browserHost != null)
                                    browserHost.Dispose();
                                browserHost = null;

                                helper = null;
                            }
                            //Start the video
                            ((MediaElement)this.FindName("FloorVideoControl")).Play();

                            //Send the screen a message
                            Utilities.MessageSender.SendMessage("GotoStart");

                            //Set the system state
                            Globals.currentAppState = RippleSystemStates.UserWaitToGoOnStart;

                            Globals.CurrentlySelectedParent = 0;

                            //Show the start options
                            ArrangeFloor();
                        }
                    }

                    else if (e.PropertyName == "SendMessage")
                    {
                        if ((!String.IsNullOrEmpty(scriptingHelper.SendMessage)))
                        {
                            //Send the screen a message for HTML parameter passing
                            Utilities.MessageSender.SendMessage(scriptingHelper.SendMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in helper property changed event {0}", ex.Message);
            }

        }

        private void UnlockRippleSystem()
        {
            //Set the system state - Updated the State
            Globals.currentAppState = RippleSystemStates.UserDetected;
            //Globals.currentAppState = RippleSystemStates.UserWaitToGoOnStart;

            //Send a message to the screen to start the system along with the user name if present
            Utilities.MessageSender.SendMessage("System Start:" + (String.IsNullOrEmpty(Globals.UserName) ? Globals.EmpAlias : Globals.UserName));

            //ArrangeFloor();
            //Wait for the screen to finish the start process for the defined duration
            waitThread = new BackgroundWorker();
            //Get the wait value
            waitDuration = floorData.Start.IntroVideoWaitPeriod;
            waitThread.DoWork += waitThread_DoWork;
            waitThread.RunWorkerCompleted += waitThread_RunWorkerCompleted;
            waitThread.RunWorkerAsync();
        }
        #endregion

        /// <summary>
        /// Function to reset the UI and take it to No User mode
        /// </summary>
        public void ResetUI()
        {
            try
            {
                //Reset the globals
                Globals.ResetGlobals();
                lastSelectedOptionName = "";
                QRMode = false;

                //Dispose the objects
                //if (player != null)
                //{
                //    //Stop the flash.
                //    player.Stop();
                //    player.Width = 0;
                //    player.Height = 0;
                //    player.Dispose();
                //    host.Dispose();
                //    player = null;
                //    host = null;
                //}

                //else 
                if (browserElement != null)
                {
                    browserElement.Dispose();
                    browserElement = null;

                    if (browserHost != null)
                        browserHost.Dispose();
                    browserHost = null;

                    helper = null;
                }

                FlashContainer.Children.Clear();

                ProjectAnimationContentOnFloor(floorData.Start.Animation.Content);

                this.UpdateLayout();

            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Reset UI {0} where floor content is {1}", ex.Message, floorData.Start.Animation.Content);
            }

        }

        void waitThread_DoWork(object sender, DoWorkEventArgs e)
        {
            //Wait in backgroud for video duration period
            System.Threading.Thread.Sleep(waitDuration * 1000);
        }

        void waitThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Set the system state
            Globals.currentAppState = RippleSystemStates.UserWaitToGoOnStart;

            //Show the floor
            ArrangeFloor();
        }

        void autoLogoutWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Wait for Auto Lock period
            System.Threading.Thread.Sleep(checkPeriodInSeconds * 1000);
        }

        void autoLogoutWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if the system is in options mode
            if (Globals.currentAppState == RippleSystemStates.Start || Globals.currentAppState == RippleSystemStates.UserWaitToGoOnStart || Globals.currentAppState == RippleSystemStates.UserPlayingAnimations || Globals.currentAppState == RippleSystemStates.OptionSelected || Globals.currentAppState == RippleSystemStates.UserDetected)
            {
                //Check when was the last user reported
                DateTime currentTime = DateTime.Now;
                if ((currentTime - Utilities.KinectHelper.lastUserVisibleTime).TotalSeconds > Convert.ToDouble(autoLockPeriodInSeconds))
                {
                    //The last time user was visible has elapsed the auto-lock period
                    //Lock the system
                    ProcessTileActionForLogout();
                }
            }

            //Keep looping either way
            autoLogoutWorker.RunWorkerAsync();
        }

        private void FloorVideoControl_MediaEnded(object sender, RoutedEventArgs e)
        {
            //Play the Ripple video
            ((MediaElement)this.FindName("FloorVideoControl")).Source = new Uri(Utilities.HelperMethods.GetAssetURI(floorData.UpperTile.Content));
            ((MediaElement)this.FindName("FloorVideoControl")).Play();
        }

        #endregion

        #region Kinect Handlers
        /// <summary>
        /// Whenever Kinect detects some location value for the skeleton or some gesture
        /// This function gets triggered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void kinectHelper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var kinectInfo = sender as Utilities.KinectHelper;
            if (kinectInfo != null)
            {
                if (e.PropertyName == "CurrentLocation")
                {
                    OnSelectedBox(kinectInfo.CurrentLocation);
                }
                else if (e.PropertyName == "KinectSwipeDetected")
                {
                    OnGestureDetected(kinectInfo.KinectGestureDetected);
                }
            }
        }

        /// <summary>
        /// On detection of hand swipe gestures
        /// </summary>
        /// <param name="type"></param>
        private void OnGestureDetected(GestureTypes type)
        {
            try
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "GestureDetected {0}", type.ToString());

                //Pass the message to the HTML content opened on the floor
                if (helper != null)
                {
                    helper.GestureReceived(type);
                }

                //To handle unlock if defined in the XML
                //Check if unlock mode is Gesture and is the current gesture defined there to unlock the system
                if (Globals.currentAppState == RippleSystemStates.NoUser && floorData.Start.Unlock.Mode == Mode.Gesture && floorData.Start.Unlock.UnlockType == type.ToString())
                {
                    UnlockRippleSystem();
                }
                #region Not needed anymore
                //To exit user animations using right swipe
                //else if (Globals.currentAppState == RippleSystemStates.UserPlayingAnimations && type == GestureTypes.RightSwipe)
                //{
                //    //Dispose the objects
                //    if (player != null)
                //    {
                //        //Stop the flash.
                //        player.Stop();
                //        player.Width = 0;
                //        player.Height = 0;
                //        player.Dispose();
                //        host.Dispose();
                //        player = null;
                //        host = null;
                //    }
                //    else if(helper != null)
                //    {
                //        //Exit the game
                //        if (helper != null)
                //        {
                //            helper.PropertyChanged -= helper_PropertyChanged;
                //            if(browserElement != null)
                //                browserElement.Dispose();
                //            browserElement = null;
                //            helper = null;
                //        }
                //    }

                //    //Start the video
                //    ((MediaElement)this.FindName("FloorVideoControl")).Play();

                //    //Send the screen a message
                //    Utilities.MessageSender.SendMessage("GotoStart");

                //    //Set the system state
                //    Globals.currentAppState = RippleSystemStates.UserWaitToGoOnStart;

                //    Globals.CurrentlySelectedParent = 0;

                //    //Show the start options
                //    ArrangeFloor();
                //} 
                #endregion
                //Check if these are gestures which need to be sent to the screen
                else if (Globals.currentAppState == RippleSystemStates.OptionSelected || Globals.currentAppState == RippleSystemStates.Start)
                {
                    Tile t = GetFloorTileForID(Globals.SelectedOptionFullName);
                    //PPT accepts left and right gestures
                    if (t.CorrespondingScreenContentType == RippleDictionary.ContentType.PPT)
                    {
                        if (type == GestureTypes.LeftSwipe || type == GestureTypes.RightSwipe)
                            Utilities.MessageSender.SendMessage("Gesture:" + type.ToString());
                    }
                    //Browser accepts zoom in, zoom out and scrolling
                    else if (t.CorrespondingScreenContentType == RippleDictionary.ContentType.HTML && t.Action != TileAction.HTML)
                    {
                        //if (type == GestureTypes.SwipeDown || type == GestureTypes.SwipeUp || type == GestureTypes.ZoomIn || type == GestureTypes.ZoomOut)
                        Utilities.MessageSender.SendMessage("Gesture:" + type.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in OnGestureDetected {0}", ex.Message);
            }
        }

        #endregion

        #region Options Code
        private void OnSelectedBox(int value)
        {
            try
            {
                //Check if the value is not relevant
                if (value < 0)
                {
                    //Reset the time stamp and do nothing
                    Globals.currentBoxTimeStamp = DateTime.Now;
                    StopAllAnimations();
                    return;
                }
                Globals.PreviouslySelectedBox = Globals.CurrentlySelectedBox;
                Globals.CurrentlySelectedBox = value;
                Globals.currentUserTimestamp = DateTime.Now;
                //Check if the same box has been selected
                if (Globals.PreviouslySelectedBox == value)
                {
                    //Check if the lockin period has elapsed
                    now = DateTime.Now;
                    if ((now - Globals.currentBoxTimeStamp).TotalSeconds >= Convert.ToDouble(floorData.LockingPeriod))
                    {
                        #region Box selected

                        //Get the tile action type
                        TileAction tileAction = TileAction.Standard;
                        //Main Option
                        if (Globals.currentAppState == RippleSystemStates.Start)
                            tileAction = floorData.Tiles["Tile" + value].Action;
                        else if (Globals.currentAppState == RippleSystemStates.OptionSelected && value != 0)
                            tileAction = floorData.Tiles["Tile" + Globals.CurrentlySelectedParent].SubTiles["Tile" + Globals.CurrentlySelectedParent + "SubTile" + value].Action;

                        //Return for static tiles
                        //Return in case the Floor is in QR Mode or if tile does nothing
                        if (tileAction == TileAction.Nothing)
                            return;

                        if (tileAction != TileAction.NothingOnFloor)
                            StartAnimationInBox(value, false);

                        //Check if same as the last selected option
                        if (lastSelectedOptionName == "Tile" + value && Globals.currentAppState != RippleSystemStates.UserWaitToGoOnStart)
                            return;

                        lastSelectedOptionName = "Tile" + value;

                        //Start selected
                        if (value == 0)
                        {
                            OnStartSelected();
                        }

                        //Boxes selected
                        //Stop tracking of other boxes during QR code mode
                        else if (!QRMode)
                        {
                            OnOptionSelected(value);
                        }
                        #endregion
                    }
                    else
                        return;
                }
                else
                {
                    //Reset the time stamp and start the animation on the floor
                    Globals.currentBoxTimeStamp = DateTime.Now;
                    StartAnimationInBox(value, true);
                }


            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in on selected box {0}", ex.Message);
            }
        }

        public void OnStartSelected()
        {
            if (Globals.currentAppState == RippleSystemStates.UserWaitToGoOnStart || Globals.currentAppState == RippleSystemStates.OptionSelected || Globals.currentAppState == RippleSystemStates.UserPlayingAnimations)
            {
                QRMode = false;
                Globals.CurrentlySelectedParent = 0;
                Globals.SelectedOptionFullName = lastSelectedOptionName;
                ShowStartOptions();
                Globals.currentAppState = RippleSystemStates.Start;
                Utilities.MessageSender.SendMessage("Tile0");
            }
        }

        public void OnOptionSelected(int BoxNumber)
        {
            try
            {
                String tileID = null;
                TileAction action;
                String g = String.Empty;

                //User selected Main Option
                if (Globals.currentAppState == RippleSystemStates.Start)
                {
                    tileID = "Tile" + BoxNumber;
                    Globals.CurrentlySelectedParent = BoxNumber;
                    Globals.currentAppState = RippleSystemStates.OptionSelected;
                    Globals.SelectedOptionFullName = tileID;

                    action = floorData.Tiles[tileID].Action;

                    //Call the tile transition for Standard Action tiles
                    if (action == TileAction.Standard)
                    {
                        //Show the main grid
                        ((Grid)this.FindName("MainOptionGrid")).Visibility = Visibility.Visible;
                        ((TextBlock)this.FindName("MainOptionGridLabel")).Text = floorData.Tiles[tileID].Name;
                        //Layout the options
                        LayoutTiles(BoxNumber);
                    }
                    else if (action == TileAction.Logout)
                    {
                        ProcessTileActionForLogout();
                    }
                    else if (action == TileAction.HTML)
                    {
                        ProcessTileActionForAnimation(floorData.Tiles[tileID].ActionURI);
                    }
                    else if (action == TileAction.QRCode)
                    {
                        //Show the main grid
                        ((Grid)this.FindName("MainOptionGrid")).Visibility = Visibility.Visible;
                        ((TextBlock)this.FindName("MainOptionGridLabel")).Text = floorData.Tiles[tileID].Name;
                        g = ProcessTileActionForQRCode(floorData.Tiles[tileID].ActionURI);
                        //No tiles would be laid out since its QR code, so even if user specifies options, it would not make sense.
                        //LayoutTiles(BoxNumber);
                    }
                    else if (action == TileAction.Nothing || action == TileAction.NothingOnFloor)
                    {
                        Globals.currentAppState = RippleSystemStates.Start;
                        Globals.CurrentlySelectedParent = 0;
                    }
                    this.UpdateLayout();

                    //Send Message to the Screen if the tile does something
                    if (action != TileAction.Logout && action != TileAction.Nothing)
                    {
                        Utilities.MessageSender.SendMessage(tileID);
                    }
                    if (QRMode)
                    {
                        //Send HTML Message to the Screen in case it is HTML
                        Utilities.MessageSender.SendMessage("HTML:SessionID," + g);
                    }

                }

                //User selected Sub Option
                else if (Globals.currentAppState == RippleSystemStates.OptionSelected)
                {
                    String parentTileID = "Tile" + Globals.CurrentlySelectedParent;
                    tileID = "Tile" + Globals.CurrentlySelectedParent + "SubTile" + BoxNumber;
                    action = floorData.Tiles[parentTileID].SubTiles[tileID].Action;
                    Globals.SelectedOptionFullName = tileID;

                    //Call the tile transition for Standard Action tiles
                    if (action == TileAction.Standard)
                    {
                        DoTileTransitionForMainOption(Globals.CurrentlySelectedParent);
                    }
                    else if (action == TileAction.Nothing || action == TileAction.NothingOnFloor)
                    { }
                    else if (action == TileAction.Logout)
                    {
                        ProcessTileActionForLogout();
                    }
                    else if (action == TileAction.HTML)
                    {
                        ProcessTileActionForAnimation(floorData.Tiles[parentTileID].SubTiles[tileID].ActionURI);
                    }
                    else if (action == TileAction.QRCode)
                    {
                        ((TextBlock)this.FindName("MainOptionGridLabel")).Text = floorData.Tiles[parentTileID].SubTiles[tileID].Name;
                        g = ProcessTileActionForQRCode(floorData.Tiles[parentTileID].SubTiles[tileID].ActionURI);
                    }

                    this.UpdateLayout();

                    //Send Message to the Screen if the tile does something
                    if (action != TileAction.Logout && action != TileAction.Nothing)
                    {
                        Utilities.MessageSender.SendMessage(tileID);
                    }

                    if (QRMode)
                    {
                        //Send HTML Message to the Screen in case it is HTML - Check with smarth on how to pass the message
                        Utilities.MessageSender.SendMessage("HTML:SessionID," + g);
                    }
                }

            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in OnOptionSelected for box number {1}: {0}", ex.Message, BoxNumber);
            }

        }

        #endregion

        #region Floor Content Processing

        public void ArrangeFloor()
        {
            try
            {
                //Dispose if the initial animation is still active, in case the unlock happened through any method except gesture or animation
                //if (floorData.Start.Animation.AnimType == AnimationType.Flash && player != null)
                //{
                //    //Stop the flash.
                //    player.Stop();
                //    player.Width = 0;
                //    player.Height = 0;
                //    player.Dispose();
                //    host.Dispose();
                //    player = null;
                //    host = null;
                //}

                //else 
                if (floorData.Start.Animation.AnimType == AnimationType.HTML && browserElement != null)
                {
                    if (browserElement != null)
                        browserElement.Dispose();
                    browserElement = null;

                    if (browserHost != null)
                        browserHost.Dispose();
                    browserHost = null;

                    helper = null;
                }

                FlashContainer.Children.Clear();
                MainContainer.Visibility = Visibility.Visible;
                FlashContainer.Visibility = Visibility.Collapsed;
                ((Grid)this.FindName("MainOptionGrid")).Visibility = Visibility.Collapsed;
                ((System.Windows.Controls.Image)this.FindName("OverlayImage")).Visibility = Visibility.Collapsed;

                //Reset the tile content
                LayoutTiles(-1);

                //Play the Ripple video after attaching the event handler
                ((MediaElement)this.FindName("FloorVideoControl")).MediaEnded += FloorVideoControl_MediaEnded;
                ((MediaElement)this.FindName("FloorVideoControl")).Source = new Uri(Utilities.HelperMethods.GetAssetURI(floorData.UpperTile.Content));
                ((MediaElement)this.FindName("FloorVideoControl")).Play();

                //Update the UI
                this.UpdateLayout();
                this.Focus();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Arrange Floor {0} {1}", ex.Message, ex.StackTrace);
            }

            //DoTileTransitionForMainOption(0);

            //Click on the screen
            HelperMethods.ClickOnFloorToGetFocus();

        }

        public void ShowStartOptions()
        {
            try
            {
                //Show the floor options
                MainContainer.Visibility = Visibility.Visible;
                FlashContainer.Visibility = Visibility.Collapsed;
                ((System.Windows.Controls.Image)this.FindName("OverlayImage")).Visibility = Visibility.Collapsed;
                ((Grid)this.FindName("MainOptionGrid")).Visibility = Visibility.Collapsed;

                LayoutTiles(0);

                //Update the UI
                this.UpdateLayout();
                this.Focus();

                //Click on the screen
                HelperMethods.ClickOnFloorToGetFocus();
            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ShowStartOptions UI {0}", ex.Message);
            }

        }

        private void LayoutTiles(int parent)
        {
            String tileID = null;
            //Reset the labels to blank to setup a blank floor
            if (parent < 0)
            {
                //Reset the labels
                ClearFloorLabels();
            }
            else if (parent == 0)
            {
                foreach (var item in floorData.Tiles.Values)
                {
                    try
                    {
                        //Set the label for the tile
                        SetAttributesForWindowsControls<TextBlock>(item.Id + Label, "Text", item.Name);

                        //Clear the inner content grid either way
                        ((Grid)this.FindName(InnerContent + item.Id)).Children.Clear();

                        //Clear the animation if any
                        try
                        {
                            var sbt = (Storyboard)this.FindName(item.Id + "SB");
                            sbt.Stop();
                        }
                        catch (Exception) { }


                        //Set the content for the tile if the tile type is not text
                        if (item.TileType != TileType.Text && (!String.IsNullOrEmpty(item.Content)))
                            ShowTileContent(item.Id, item.TileType, Utilities.HelperMethods.GetAssetURI(item.Content));
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }
                }
            }
            else if (parent > 0)
            {
                foreach (var item in floorData.Tiles["Tile" + parent].SubTiles.Values)
                {
                    try
                    {
                        tileID = item.Id.Substring(item.Id.LastIndexOf("Tile"));

                        //Set the label for the tile
                        SetAttributesForWindowsControls<TextBlock>(tileID + Label, "Text", item.Name);

                        //Clear the inner content grid either way
                        ((Grid)this.FindName(InnerContent + tileID)).Children.Clear();

                        //Clear the animation if any
                        try
                        {
                            var sbt = (Storyboard)this.FindName(tileID + "SB");
                            sbt.Stop();
                        }
                        catch (Exception) { }

                        //Set the content for the tile if the tile type is not text
                        if (item.TileType != TileType.Text && (!String.IsNullOrEmpty(item.Content)))
                            ShowTileContent(tileID, item.TileType, Utilities.HelperMethods.GetAssetURI(item.Content));
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }
                }
            }
            this.UpdateLayout();

            //Call tile transitions
            DoTileTransitionForMainOption(Globals.CurrentlySelectedParent);
        }

        private void ShowTileContent(String tileID, TileType tileType, String contentURI)
        {
            try
            {
                switch (tileType)
                {
                    case TileType.OnlyMedia:
                        //Check the content type
                        //Image
                        if (contentURI.Contains("\\Assets\\Images\\"))
                        {
                            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                            img.Source = new BitmapImage(new Uri(contentURI));
                            img.Style = (System.Windows.Style)App.Current.FindResource("TileImageStyle");
                            ((Grid)this.FindName(InnerContent + tileID)).Children.Add(img);
                        }
                        //Video
                        else if (contentURI.Contains("\\Assets\\Videos\\"))
                        {
                            MediaElement video = new MediaElement();
                            video.Source = new Uri(contentURI);
                            ((Grid)this.FindName(InnerContent + tileID)).Children.Add(video);
                            video.Play();
                        }
                        break;
                    case TileType.TextThumbnail:
                        System.Windows.Controls.Image thum_img = new System.Windows.Controls.Image();
                        thum_img.Source = new BitmapImage(new Uri(contentURI));
                        thum_img.Style = (System.Windows.Style)App.Current.FindResource("ThumbnailImageStyle");
                        ((Grid)this.FindName(InnerContent + tileID)).Children.Add(thum_img);
                        break;
                    case TileType.LiveTile:
                        System.Windows.Controls.Image tile_img = new System.Windows.Controls.Image();
                        tile_img.Name = "ImageAndText" + InnerContent + tileID;
                        tile_img.Source = new BitmapImage(new Uri(contentURI));
                        tile_img.Style = (System.Windows.Style)App.Current.FindResource("TileImageStyle");
                        ((Grid)this.FindName(InnerContent + tileID)).Children.Add(tile_img);
                        this.UpdateLayout();
                        //Add live tile animation
                        //Set for image
                        Storyboard sb = liveTile.Clone();
                        sb.Name = tileID + "SB";
                        try
                        {
                            this.RegisterName(sb.Name, sb);
                        }
                        catch (Exception)
                        {

                            this.UnregisterName(sb.Name);
                            this.RegisterName(sb.Name, sb);
                        }
                        Storyboard.SetTarget(sb.Children[0], tile_img);
                        Storyboard.SetTarget(sb.Children[1], ((TextBlock)this.FindName(tileID + Label)));
                        Storyboard.SetTarget(sb.Children[2], ((TextBlock)this.FindName(tileID + Label)));
                        //Set keytimes to random values
                        //double val = rnd.NextDouble() * 2;
                        ////round off
                        //val = Math.Round(val, 2);
                        //TimeSpan tp = TimeSpan.FromSeconds(val);
                        //KeyTime newVal = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        //foreach (EasingDoubleKeyFrame sbItem in ((DoubleAnimationUsingKeyFrames)sb.Children[0]).KeyFrames)
                        //{
                        //    newVal = KeyTime.FromTimeSpan(sbItem.KeyTime.TimeSpan + tp);
                        //    sbItem.SetValue(EasingDoubleKeyFrame.KeyTimeProperty, newVal);
                        //}
                        //foreach (EasingDoubleKeyFrame sbItem in ((DoubleAnimationUsingKeyFrames)sb.Children[1]).KeyFrames)
                        //{
                        //    newVal = KeyTime.FromTimeSpan(sbItem.KeyTime.TimeSpan + tp);
                        //    sbItem.SetValue(EasingDoubleKeyFrame.KeyTimeProperty, newVal);
                        //}
                        //foreach (DiscreteObjectKeyFrame sbItem in ((ObjectAnimationUsingKeyFrames)sb.Children[2]).KeyFrames)
                        //{
                        //    newVal = KeyTime.FromTimeSpan(sbItem.KeyTime.TimeSpan + tp);
                        //    sbItem.SetValue(DiscreteObjectKeyFrame.KeyTimeProperty, newVal);
                        //}
                        sb.Begin();
                        break;
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ShowTileContent for TileID {0} and content URI {2} : {1}", tileID, ex.Message, contentURI);
            }
        }

        private void ClearFloorLabels()
        {
            foreach (var item in floorData.Tiles.Values)
            {
                SetAttributesForWindowsControls<TextBlock>(item.Id + Label, "Text", "");
                ((Grid)this.FindName(InnerContent + item.Id)).Children.Clear();
            }

            //Set the start label
            ((TextBlock)this.FindName("Tile0Label")).Text = floorData.Tiles["Tile0"].Name;
        }

        #region TileActions
        private void ProcessTileActionForAnimation(string actionURI)
        {
            try
            {
                //Set the application state
                Globals.currentAppState = RippleSystemStates.UserPlayingAnimations;

                //Stop the video
                ((MediaElement)this.FindName("FloorVideoControl")).Stop();

                //Project the HTML or flash content onto the floor
                ProjectAnimationContentOnFloor(actionURI);
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ProcessTileActionForAnimation for URI {1}: {0}", ex.Message, actionURI);
            }
        }

        private String ProcessTileActionForQRCode(string actionURI)
        {
            try
            {
                //Set the mode
                QRMode = true;

                //TODO = Hide other options
                ClearFloorLabels();

                //Show QR Code on the floor
                //Add session Hint to QRCode - TODO Later 
                Guid g = Guid.NewGuid();
                String URL = actionURI + "?SessionID=" + g.ToString();
                Bitmap bitmap = RippleCommonUtilities.HelperMethods.GenerateQRCode(URL);
                BitmapImage bitmapImage;
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }

                ((System.Windows.Controls.Image)this.FindName("OverlayImage")).Visibility = Visibility.Visible;
                ((System.Windows.Controls.Image)this.FindName("OverlayImage")).Source = bitmapImage;

                DoTileTransitionForMainOption(Globals.CurrentlySelectedParent);

                return g.ToString();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in QR code generation {0}", ex.Message);
                return String.Empty;
            }
        }

        private void ProcessTileActionForLogout()
        {
            ResetUI();

            //Send message to the screen
            Utilities.MessageSender.SendMessage("Reset");
        }

        private void ProjectAnimationContentOnFloor(string actionUri)
        {
            try
            {
                //Set the visibility
                MainContainer.Visibility = Visibility.Collapsed;
                FlashContainer.Visibility = Visibility.Visible;

                //if (Path.GetExtension(actionUri).ToLower().Equals(".swf"))
                //{
                //    //Play flash animation
                //    host = new WindowsFormsHost();
                //    player = new FlashAxControl();
                //    host.Child = player;
                //    FlashContainer.Children.Clear();
                //    FlashContainer.Children.Add(host);

                //    //set size - based on the resolution
                //    player.Width = (int)Globals.CurrentResolution.HorizontalResolution;
                //    player.Height = (int)Globals.CurrentResolution.VerticalResolution;
                //    //load & play the movie
                //    player.LoadMovie(Utilities.HelperMethods.GetAssetURI(actionUri));
                //    player.Play();

                //    //Get the window focus
                //    this.Focus();
                //}
                //else
                {
                    //Play HTML animations
                    browserHost = new WindowsFormsHost();
                    browserElement = new System.Windows.Forms.WebBrowser();
                    browserElement.ScriptErrorsSuppressed = true;
                    helper = new Utilities.ScriptingHelper(this);
                    browserHost.Child = browserElement;
                    helper.PropertyChanged += helper_PropertyChanged;
                    browserElement.ObjectForScripting = helper;
                    FlashContainer.Children.Clear();
                    FlashContainer.Children.Add(browserHost);
                    String pageUri = String.Empty;
                    String fileLocation = Utilities.HelperMethods.GetAssetURI(actionUri);
                    //Local file
                    if (File.Exists(fileLocation))
                    {
                        String[] PathParts = fileLocation.Split(new char[] { ':' });
                        pageUri = "file://127.0.0.1/" + PathParts[0] + "$" + PathParts[1];
                    }
                    //Web hosted file
                    else
                    {
                        pageUri = actionUri;
                    }
                    browserElement.Navigate(pageUri);
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ProjectAnimationContentOnFloor for URI {1}: {0}", ex.Message, actionUri);
            }
        }

        #endregion

        #endregion

        #region Color and Animation Helpers
        /// <summary>
        /// Code to start animation on the box, for the locking period
        /// </summary>
        /// <param name="value"></param>
        public void StartAnimationInBox(int BoxNumber, bool IsAnimation)
        {
            //Reset all the floor and start animation only on box "value"
            StopAllAnimations();

            //Get the tile action type
            TileAction tileAction = TileAction.Standard;
            //Main Option
            if (Globals.currentAppState == RippleSystemStates.Start)
                tileAction = floorData.Tiles["Tile" + BoxNumber].Action;
            else if (Globals.currentAppState == RippleSystemStates.OptionSelected && BoxNumber != 0)
                tileAction = floorData.Tiles["Tile" + Globals.CurrentlySelectedParent].SubTiles["Tile" + Globals.CurrentlySelectedParent + "SubTile" + BoxNumber].Action;

            //Return for static tiles
            //Return in case the Floor is in QR Mode or if tile does nothing
            if (tileAction == TileAction.Nothing || tileAction == TileAction.NothingOnFloor || QRMode)
                return;

            ((Grid)this.FindName("Tile" + BoxNumber)).Background = IsAnimation ? new SolidColorBrush(Colors.Yellow) : new SolidColorBrush(Colors.Red);
        }

        public void StopAllAnimations()
        {
            ResetColorsForMainOption(Globals.CurrentlySelectedParent);

            //Update the UI
            this.UpdateLayout();
        }

        private void DoTileTransitionForMainOption(int Parent)
        {
            try
            {
                #region AnimateFloor
                ((Grid)this.FindName("Tile0")).Background = new SolidColorBrush(floorData.Tiles["Tile0"].Color);
                List<Tile> tileList = null;

                //Set the colors
                if (Parent > 0)
                {
                    ((Grid)this.FindName("MainOptionGrid")).Background = new SolidColorBrush(floorData.Tiles["Tile" + Parent].Color);
                    tileList = floorData.Tiles["Tile" + Parent].SubTiles.Values.ToList<Tile>();
                }
                else if (Parent == 0)
                {
                    tileList = floorData.Tiles.Values.ToList<Tile>();
                }


                int val = 0;
                EasingColorKeyFrame kFrame;
                foreach (var item in tileList)
                {
                    val = (Convert.ToInt32(item.Id.Substring(item.Id.LastIndexOf('e') + 1)) * 2) - 1;
                    if (val >= 0)
                    {
                        kFrame = (EasingColorKeyFrame)((ColorAnimationUsingKeyFrames)tileTransitionSB.Children[val]).KeyFrames[0].Clone();
                        kFrame.Value = item.Color;
                        ((ColorAnimationUsingKeyFrames)tileTransitionSB.Children[val]).KeyFrames[0] = kFrame;
                    }

                }

                //Play the music
                using (var soundPlayer = new SoundPlayer(Utilities.HelperMethods.GetAssetURI(floorData.Transition.Music)))
                {
                    soundPlayer.Play(); // can also use soundPlayer.PlaySync()
                }

                //Start the tile transition
                tileTransitionSB.Begin();

                #endregion
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in DoTileTransition for parent {0}: {1}", Parent, ex.Message);
            }

        }

        private void ResetColorsForMainOption(int Parent)
        {
            try
            {
                //Set teh background value for Start
                ((Grid)this.FindName("Tile0")).Background = new SolidColorBrush(floorData.Tiles["Tile0"].Color);
                //Set the colors
                if (Parent > 0)
                {
                    ((Grid)this.FindName("MainOptionGrid")).Background = new SolidColorBrush(floorData.Tiles["Tile" + Parent].Color);

                    foreach (var item in floorData.Tiles["Tile" + Parent].SubTiles.Values)
                    {
                        SetAttributesForWindowsControls<Grid>(item.Id.Substring(item.Id.LastIndexOf("Tile")), "Background", new SolidColorBrush(item.Color));
                    }
                }
                else if (Parent == 0)
                {
                    foreach (var item in floorData.Tiles.Values)
                    {
                        SetAttributesForWindowsControls<Grid>(item.Id, "Background", new SolidColorBrush(item.Color));
                    }
                }

                this.UpdateLayout();
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ResetColorsForMainOption {0}, {1}", Parent, ex.Message);
            }
        }
        #endregion

        #region Helpers

        private Tile GetFloorTileForID(string TileID)
        {
            Tile reqdTile = null;
            try
            {
                reqdTile = floorData.Tiles[TileID];
            }
            catch (Exception)
            {
                try
                {
                    reqdTile = floorData.Tiles[TileID.Substring(0, TileID.LastIndexOf("SubTile"))].SubTiles[TileID];
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return reqdTile;
        }

        private void SetAttributesForWindowsControls<InstanceType>(string objectName, string propertyName, object propertyValue)
        {
            InstanceType objectInstanceType = (InstanceType)this.FindName(objectName);

            if (objectInstanceType != null)
            {
                System.Reflection.PropertyInfo prop = typeof(InstanceType).GetProperty(propertyName);
                prop.SetValue(objectInstanceType, propertyValue, null);
            }
        }

        #endregion

        private void FloorWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            ResetUI();
            //Globals.currentAppState = RippleSystemStates.UserDetected;
            //ArrangeFloor();
            //Globals.currentAppState = RippleSystemStates.UserWaitToGoOnStart;
            //ShowStartOptions();
            //Globals.currentAppState = RippleSystemStates.Start;
            //Globals.CurrentlySelectedParent = 5;
            //OnOptionSelected(5);
            //BackgroundWorker vf = new BackgroundWorker();
            //vf.DoWork += vf_DoWork;
            //vf.RunWorkerCompleted += vf_RunWorkerCompleted;
            //vf.RunWorkerAsync();

            ////Globals.CurrentlySelectedParent = 7;
            ////Globals.SelectedOptionFullName = "Tile7";
            //OnGestureDetected(GestureTypes.RightSwipe);
            //////OnStartSelected();
            ////OnOptionSelected(8);
        }
    }
}
