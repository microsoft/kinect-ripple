using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.ComponentModel;
using RippleCommonUtilities;
using System.Windows;
using System.Windows.Input;
using System.Timers;
using RippleDictionary;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;

namespace RippleFloorApp.Utilities
{
    public class KinectHelper : INotifyPropertyChanged
    {
        /// <summary>
        /// The recognizer being used.
        /// </summary>
        public static Recognizer activeRecognizer;
        private Skeleton[] _FrameSkeletons;
        private KinectSensor sensor;
        private int currentLocation;
        private RippleSystemStates currentState;
        private String kinectConnectionState;
        private GestureTypes kinectGestureDetected;

        private static double frontDistance = 0.00;
        private static double backDistance = 0.00;
        private static double leftDistance = 0.00;
        private static double rightDistance = 0.00;

        private static double[] tileOriginX;
        private static double[] tileOriginY;

        private static double[] tileWidth;
        private static double[] tileHeight;

        private static double[] floorWidth;
        private static double[] floorHeight;

        private static double[] leftTileBoundary;
        private static double[] topTileBoundary;

        private static double[] rightTileBoundary;
        private static double[] bottomTileBoundary;


        public static int tileCount = 0;

        public static DateTime lastUserVisibleTime = new DateTime();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public int CurrentLocation
        {
            get { return currentLocation; }
            set
            {
                currentLocation = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentLocation"));

            }
        }

        public RippleSystemStates CurrentState
        {
            get { return currentState; }
            set
            {
                currentState = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentState"));
            }
        }

        public KinectHelper()
        {
            try
            {
                //Get the callibration data
                CalibrationConfiguration config = RippleDictionary.Dictionary.GetFloorConfigurations(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
                frontDistance = config.FrontDistance;
                backDistance = config.BackDistance;
                leftDistance = config.LeftDistance;
                rightDistance = config.RightDistance;


                //Get the floor tile data
                tileCount = FloorWindow.floorData.Tiles.Count;

                tileOriginX = new double[tileCount];
                tileOriginY = new double[tileCount];

                tileWidth = new double[tileCount];
                tileHeight = new double[tileCount];

                floorWidth = new double[tileCount];
                floorHeight = new double[tileCount];

                leftTileBoundary = new double[tileCount];
                topTileBoundary = new double[tileCount];

                rightTileBoundary = new double[tileCount];
                bottomTileBoundary = new double[tileCount];

                for (int i = 0; i < tileCount; i++)
                {
                    // Origin at top left (towards the side of the video)
                    tileOriginX[i] = FloorWindow.floorData.Tiles["Tile" + i].Coordinate.X;
                    tileOriginY[i] = FloorWindow.floorData.Tiles["Tile" + i].Coordinate.Y;

                    tileWidth[i] = FloorWindow.floorData.Tiles["Tile" + i].Style.Width;
                    tileHeight[i] = FloorWindow.floorData.Tiles["Tile" + i].Style.Height;

                    floorWidth[i] = rightDistance - leftDistance;
                    floorHeight[i] = backDistance - frontDistance;

                    leftTileBoundary[i] = leftDistance + tileOriginX[i] * floorWidth[i];
                    topTileBoundary[i] = frontDistance + tileOriginY[i] * floorHeight[i];

                    rightTileBoundary[i] = leftDistance + tileOriginX[i] * floorWidth[i] + tileWidth[i] * floorWidth[i];
                    bottomTileBoundary[i] = frontDistance + tileOriginY[i] * floorHeight[i] + tileHeight[i] * floorHeight[i];
                }


                if (KinectSensor.KinectSensors.Count > 0)
                {
                    KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
                    this.sensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                    if (this.sensor != null)
                    {
                        Initialize();
                    }
                    else
                    {
                        KinectConnectionState = "Disconnected";
                    }
                }
                else
                {
                    KinectConnectionState = "Disconnected";
                }
                activeRecognizer = CreateRecognizer();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Event Handlers

        public String KinectConnectionState
        {
            get { return kinectConnectionState; }
            set
            {
                kinectConnectionState = value;
                OnPropertyChanged(new PropertyChangedEventArgs("KinectConnectionState"));
            }
        }

        public GestureTypes KinectSwipeDetected
        {
            get { return kinectGestureDetected; }
            set
            {
                kinectGestureDetected = value;
                OnPropertyChanged(new PropertyChangedEventArgs("KinectSwipeDetected"));
            }
        }

        void Initialize()
        {
            if (sensor != null)
            {
                this.sensor.SkeletonStream.Enable();
                this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
                this._FrameSkeletons = new Skeleton[this.sensor.SkeletonStream.FrameSkeletonArrayLength];
                sensor.Start();
                KinectConnectionState = "Connected";
            }
        }

        void Unitialize()
        {
            if (sensor != null)
            {
                sensor.Stop();
                this.sensor.SkeletonStream.Disable();
                this.sensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                this.sensor = null;
            }
        }

        #region Improve the code

        #endregion

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Joint Spine = new Joint();
            Joint LeftFeet = new Joint();
            Joint RightFeet = new Joint();
            float delta = 0.00f;
            float leftFeetXPosition = 0.00f;
            float rightFeetXPosition = 0.00f;

            try
            {
                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame != null)
                    {
                        frame.CopySkeletonDataTo(this._FrameSkeletons);

                        Skeleton skeleton = GetPrimarySkeleton(this._FrameSkeletons);

                        if (skeleton != null)
                        {

                            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                delta = 0.00f;
                                Spine = skeleton.Joints[JointType.Spine];
                                LeftFeet = skeleton.Joints[JointType.FootLeft];
                                RightFeet = skeleton.Joints[JointType.FootRight];
                                leftFeetXPosition = LeftFeet.Position.X - delta;
                                rightFeetXPosition = RightFeet.Position.X + delta;
                            }
                            else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                            {
                                delta = 0.08f;
                                LeftFeet.Position = skeleton.Position;
                                RightFeet.Position = skeleton.Position;
                                Spine.Position = skeleton.Position;
                                leftFeetXPosition = LeftFeet.Position.X - delta;
                                rightFeetXPosition = RightFeet.Position.X + delta;
                            }

                            if (skeleton.TrackingState == SkeletonTrackingState.Tracked || skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                            {
                                //Recognize Gestures either way

                                //User being detected
                                lastUserVisibleTime = DateTime.Now;

                                if (Globals.currentAppState == RippleSystemStates.UserPlayingAnimations || Globals.currentAppState == RippleSystemStates.NoUser || Globals.currentAppState == RippleSystemStates.UserDetected)
                                {
                                    //Run Mouse Interop
                                    #region Calibrated MouseInterop
                                    if ((LeftFeet.Position.Z > frontDistance && LeftFeet.Position.Z < backDistance) && (LeftFeet.Position.X > (leftDistance) && LeftFeet.Position.X < rightDistance))
                                    {
                                        double CursorX = (((LeftFeet.Position.Z + RightFeet.Position.Z) / 2 - (frontDistance)) / (backDistance - frontDistance)) * Globals.CurrentResolution.HorizontalResolution;
                                        CursorX = Globals.CurrentResolution.HorizontalResolution - CursorX;
                                        double CursorY = (((LeftFeet.Position.X + RightFeet.Position.X) / 2 - (leftDistance)) / (rightDistance - leftDistance)) * Globals.CurrentResolution.VerticalResolution;
                                        int x = Convert.ToInt32(CursorX);
                                        int y = Convert.ToInt32(CursorY);
                                        Mouse.OverrideCursor = Cursors.None;

                                        //if (count == 0)
                                        //{
                                        //    RippleCommonUtilities.OSNativeMethods.SendMouseInput(x, y, (int)Globals.CurrentResolution.HorizontalResolution, (int)Globals.CurrentResolution.VerticalResolution, true);
                                        //    count = 10;
                                        //}
                                        //count--;
                                        //RippleCommonUtilities.OSNativeMethods.SendMouseInput(x, y, (int)Globals.CurrentResolution.HorizontalResolution, (int)Globals.CurrentResolution.VerticalResolution, true);
                                        RippleCommonUtilities.OSNativeMethods.SendMouseInput(x, y, (int)Globals.CurrentResolution.HorizontalResolution, (int)Globals.CurrentResolution.VerticalResolution, false);
                                    }
                                    #endregion
                                }
                                //Run block identification only if not in above mode
                                else
                                {
                                    #region Calibrated Tile Detection

                                    bool locationChanged = false;
                                    for (int i = 0; i < tileCount; i++)
                                    {
                                        if ((LeftFeet.Position.Z > topTileBoundary[i] && LeftFeet.Position.Z < bottomTileBoundary[i]) && (RightFeet.Position.Z > topTileBoundary[i] && RightFeet.Position.Z < bottomTileBoundary[i]) && (leftFeetXPosition > (leftTileBoundary[i]) && leftFeetXPosition < rightTileBoundary[i]) && (rightFeetXPosition > (leftTileBoundary[i]) && rightFeetXPosition < rightTileBoundary[i]))
                                        {
                                            CurrentLocation = i;
                                            locationChanged = true;
                                            i = 0;
                                            break;
                                        }
                                    }
                                    if (locationChanged == false)
                                    {
                                        CurrentLocation = -1;
                                    }

                                    #endregion
                                }
                            }
                            else
                            {
                                CurrentLocation = -1;
                            }
                            activeRecognizer.Recognize(sender, frame, this._FrameSkeletons);

                        }
                        else
                        {
                            CurrentLocation = -1;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //Do nothing
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Kinect helper {0}", ex.Message);
            }


        }

        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {

            Skeleton skeleton = null;
            if (skeletons != null)
            {
                // Find the closest skeleton
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked || skeletons[i].TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        //Valid Skeleton detected
                        #region Calibrated Boundary
                        if (((skeletons[i].Position.Z > frontDistance && skeletons[i].Position.Z < backDistance) && (skeletons[i].Position.X > (leftDistance) && skeletons[i].Position.X < rightDistance)))
                        {
                            if (skeleton != null)
                            {
                                if (((skeleton.Position.Z > frontDistance && skeleton.Position.Z < backDistance) && (skeleton.Position.X > (leftDistance) && skeleton.Position.X < rightDistance)))
                                {
                                    if (skeleton.Position.Z > skeletons[i].Position.Z)
                                        skeleton = skeletons[i];
                                }
                            }
                            else
                            {
                                skeleton = skeletons[i];
                            }

                        }
                        #endregion
                    }
                }
            }
            return skeleton;
        }

        #endregion Event Handlers

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    this.sensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                    KinectConnectionState = "Connected";
                    Initialize();
                    break;

                case KinectStatus.Disconnected:
                    KinectConnectionState = "Disconnected";
                    Unitialize();
                    break;
                case KinectStatus.NotPowered:

                    KinectConnectionState = "Kinect Not Powered";
                    Unitialize();
                    break;


            }
        }

        /// <summary>
        /// Create a wired-up recognizer for running the slideshow.
        /// </summary>
        /// <returns>The wired-up recognizer.</returns>
        private Recognizer CreateRecognizer()
        {
            // Instantiate a recognizer.
            var recognizer = new Recognizer();
            // Wire-up swipe right to manually advance picture.
            recognizer.SwipeRightDetected += (s, e) =>
            {
                KinectSwipeDetected = GestureTypes.RightSwipe;
            };

            // Wire-up swipe left to manually reverse picture.
            recognizer.SwipeLeftDetected += (s, e) =>
            {
                KinectSwipeDetected = GestureTypes.LeftSwipe;
            };

            return recognizer;
        }

    }
}
