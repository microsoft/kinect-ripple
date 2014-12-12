using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using System.ComponentModel;
using RippleCommonUtilities;
using System.Windows.Input;
using System.Timers;
using RippleDictionary;
using RippleFloorApp.Utilities.KinectGestures;
using RippleFloorApp.Utilities.KinectGestures.Segments;

namespace RippleFloorApp.Utilities
{
    public class KinectHelper : INotifyPropertyChanged
    {
        /// <summary>
        /// Reader for body frames
        /// </summary>
        BodyFrameReader reader;

        /// <summary>
        /// Array for the bodies        /// </summary>
        Body[] bodies;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
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

        // skeleton gesture recognizer
        private GestureController gestureController;

        Timer _clearTimer;

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


                //if (KinectSensor.KinectSensors.Count > 0)
                //{

                    this.sensor = KinectSensor.GetDefault();
                    if (this.sensor != null)
                    {
                        Initialize();
                    }
                    else
                    {
                        KinectConnectionState = "Disconnected";
                    }
                //}
                //else
                //{
                //    KinectConnectionState = "Disconnected";
                //}
                // activeRecognizer = CreateRecognizer();
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

        public GestureTypes KinectGestureDetected
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
            if (this.sensor != null)
            {
                // open the sensor
                this.sensor.Open();

                //bodies = new Body[sensor.BodyFrameSource.BodyCount];

                // open the reader for the body frames
                reader = sensor.BodyFrameSource.OpenReader();

                if (this.reader != null)
                {
                    this.reader.FrameArrived += this.sensor_SkeletonFrameReady;
                }


                KinectConnectionState = "Connected";
                // add timer for clearing last detected gesture
                _clearTimer = new Timer(2000);
                _clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);
                // initialize the gesture recognizer
                gestureController = new GestureController();
                gestureController.GestureRecognized += OnGestureRecognized;

                // register the gestures for this demo
                RegisterGestures();
            }
            //if (sensor != null)
            //{
            //    this.sensor.SkeletonStream.Enable();
            //    this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
            //    this._FrameSkeletons = new Skeleton[this.sensor.SkeletonStream.FrameSkeletonArrayLength];
            //    sensor.Start();
            //    KinectConnectionState = "Connected";
            //}

        }

        void Unitialize()
        {
            if (this.reader != null)
            {
                // BodyFrameReder is IDisposable
                this.reader.Dispose();
                this.reader = null;
            }

            // Body is IDisposable
            if (this.bodies != null)
            {
                bodies = null;
                //foreach (Body body in this.bodies)
                //{
                //    if (body != null)
                //    {
                //        body.Dispose();
                //    }
                //}
            }

            if (this.sensor != null)
            {
                this.sensor.Close();
                this.sensor = null;
            }

            if (gestureController != null)
            {
                gestureController.GestureRecognized -= OnGestureRecognized;
                gestureController = null;
            }
        }

        #region Improve the code
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Gesture event arguments.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            switch (e.GestureName)
            {
                case "JoinedHands":
                    //KinectSwipeDetected = GestureTypes.JoinedHands;
                    break;
                case "SwipeLeft":
                    KinectGestureDetected = GestureTypes.RightSwipe;
                    break;
                case "SwipeRight":
                    KinectGestureDetected = GestureTypes.LeftSwipe;
                    break;
                case "SwipeUp":
                    //KinectSwipeDetected = GestureTypes.SwipeUp;
                    break;
                default:
                    break;
            }

            _clearTimer.Start();
        }

        /// <summary>
        /// Helper function to register all available 
        /// </summary>
        private void RegisterGestures()
        {

            IRelativeGestureSegment[] joinedhandsSegments = new IRelativeGestureSegment[20];
            JoinedHandsSegment1 joinedhandsSegment = new JoinedHandsSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 10 times 
                joinedhandsSegments[i] = joinedhandsSegment;
            }
            gestureController.AddGesture("JoinedHands", joinedhandsSegments);

            IRelativeGestureSegment[] swipeleftSegments = new IRelativeGestureSegment[3];
            swipeleftSegments[0] = new SwipeLeftSegment1();
            swipeleftSegments[1] = new SwipeLeftSegment2();
            swipeleftSegments[2] = new SwipeLeftSegment3();
            gestureController.AddGesture("SwipeLeft", swipeleftSegments);

            IRelativeGestureSegment[] swiperightSegments = new IRelativeGestureSegment[3];
            swiperightSegments[0] = new SwipeRightSegment1();
            swiperightSegments[1] = new SwipeRightSegment2();
            swiperightSegments[2] = new SwipeRightSegment3();
            gestureController.AddGesture("SwipeRight", swiperightSegments);

            IRelativeGestureSegment[] swipeUpSegments = new IRelativeGestureSegment[3];
            swipeUpSegments[0] = new SwipeUpSegment1();
            swipeUpSegments[1] = new SwipeUpSegment2();
            swipeUpSegments[2] = new SwipeUpSegment3();
            gestureController.AddGesture("SwipeUp", swipeUpSegments);

        }

        /// <summary>
        /// Clear text after some time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            kinectGestureDetected = GestureTypes.None;
            _clearTimer.Stop();
        }
        #endregion

        void sensor_SkeletonFrameReady(object sender, BodyFrameArrivedEventArgs e)
        {
            Joint Spine = new Joint();
            Joint LeftFeet = new Joint();
            Joint RightFeet = new Joint();
            float delta = 0.00f;
            float leftFeetXPosition = 0.00f;
            float rightFeetXPosition = 0.00f;

            try
            {
                BodyFrameReference frameReference = e.FrameReference;

                BodyFrame frame = frameReference.AcquireFrame();


                if (frame != null)
                {
                    // BodyFrame is IDisposable
                    using (frame)
                    {
                        if (this.bodies == null)
                        {
                            this.bodies = new Body[frame.BodyCount];
                        }
                        // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                        // As long as those body objects are not disposed and not set to null in the array,
                        // those body objects will be re-used.
                        frame.GetAndRefreshBodyData(this.bodies);

                        Body skeleton = GetPrimarySkeleton(this.bodies);

                        if (skeleton != null)
                        {

                            if (skeleton.IsTracked)
                            {
                                delta = 0.00f;
                                Spine = skeleton.Joints[JointType.SpineMid];
                                LeftFeet = skeleton.Joints[JointType.FootLeft];
                                RightFeet = skeleton.Joints[JointType.FootRight];
                                leftFeetXPosition = LeftFeet.Position.X - delta;
                                rightFeetXPosition = RightFeet.Position.X + delta;
                            }
                            //else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                            //{
                            //    delta = 0.08f;
                            //    LeftFeet.Position = skeleton.Position;
                            //    RightFeet.Position = skeleton.Position;
                            //    Spine.Position = skeleton.Position;
                            //    leftFeetXPosition = LeftFeet.Position.X - delta;
                            //    rightFeetXPosition = RightFeet.Position.X + delta;
                            //}

                            if (skeleton.IsTracked)
                            {
                                //Recognize Gestures either way
                                // update the gesture controller
                                gestureController.UpdateAllGestures(skeleton);

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

        private static Body GetPrimarySkeleton(Body[] skeletons)
        {

            Body skeleton = null;
            if (skeletons != null)
            {
                // Find the closest skeleton
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].IsTracked)
                    {
                        IReadOnlyDictionary<JointType, Joint> joints = skeletons[i].Joints;
                        double averagefeetdistanceZ = ((joints[JointType.FootLeft].Position.Z + joints[JointType.FootRight].Position.Z) / 2);
                        double averagefeetdistanceX = ((joints[JointType.FootLeft].Position.X + joints[JointType.FootRight].Position.X) / 2);

                        #region Calibrated Boundary
                        if ((averagefeetdistanceZ > frontDistance && averagefeetdistanceZ < backDistance) && (averagefeetdistanceX > leftDistance && averagefeetdistanceX < rightDistance))
                        {
                            if (skeleton != null)
                            {
                                IReadOnlyDictionary<JointType, Joint> jointsold = skeleton.Joints;
                                double averagefeetdistanceoldZ = ((jointsold[JointType.FootLeft].Position.Z + jointsold[JointType.FootRight].Position.Z) / 2);
                                double averagefeetdistanceoldX = ((jointsold[JointType.FootLeft].Position.X + jointsold[JointType.FootRight].Position.X) / 2);

                                if (((averagefeetdistanceoldZ > frontDistance && averagefeetdistanceoldZ < backDistance) && (averagefeetdistanceoldX > (leftDistance) && averagefeetdistanceoldX < rightDistance)))
                                {
                                    if (averagefeetdistanceoldZ > averagefeetdistanceZ)
                                        skeleton = skeletons[i];
                                }
                            }
                            else
                            {
                                skeleton = skeletons[i];
                            }

                        }
                        #endregion
                        //    }
                    }
                }
            }
            return skeleton;
        }

        #endregion Event Handlers

    }
}
