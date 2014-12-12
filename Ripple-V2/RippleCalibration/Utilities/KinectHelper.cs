using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Timers;
using RippleCalibration.Utilities.Gestures;
using RippleCalibration.Utilities.Gestures.Segments;

namespace RippleCalibration.Utilities
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

        private String kinectGestureDetected;
        private int currentLocation;
        private String kinectConnectionState;
        private String kinectSwipeDetected;
        // skeleton gesture recognizer
        private GestureController gestureController;
        int count = 0;

        Timer _clearTimer;

        /// <summary>
        /// Gets or sets the last recognized gesture.
        /// </summary>
        private string _gesture;
        public String Gesture
        {
            get { return _gesture; }

            private set
            {
                if (_gesture == value)
                    return;

                _gesture = value;

                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
            }
        }
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
                case "SwipeRight":
                    KinectSwipeDetected = "LeftSwipe";
                    break;
                case "SwipeLeft":
                    KinectSwipeDetected = "RightSwipe";
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
            // define the gestures for the demo

            IRelativeGestureSegment[] swiperightSegments = new IRelativeGestureSegment[3];
            swiperightSegments[0] = new SwipeRightSegment1();
            swiperightSegments[1] = new SwipeRightSegment2();
            swiperightSegments[2] = new SwipeRightSegment3();
            gestureController.AddGesture("SwipeRight", swiperightSegments);

            IRelativeGestureSegment[] swipeleftSegments = new IRelativeGestureSegment[3];
            swipeleftSegments[0] = new SwipeLeftSegment1();
            swipeleftSegments[1] = new SwipeLeftSegment2();
            swipeleftSegments[2] = new SwipeLeftSegment3();
            gestureController.AddGesture("SwipeLeft", swipeleftSegments);


        }

        /// <summary>
        /// Clear text after some time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            kinectGestureDetected = "None";
            _clearTimer.Stop();
        }
        #endregion

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

        public String KinectSwipeDetected
        {
            get { return kinectSwipeDetected; }
            set
            {
                kinectSwipeDetected = value;
                OnPropertyChanged(new PropertyChangedEventArgs("KinectSwipeDetected"));
                if (kinectSwipeDetected.Contains("Right"))
                {
                    Globals.setCoordinates = true;
                }
            }
        }

        public void SetCoordinates(double averageWidth, double averageLength)
        {
            if (Globals.appState == AppState.firstCoordinate)
            {
                bool valuesCheck = ValuesCheck();
                if (valuesCheck)
                {
                    Globals.X3 = averageLength;
                    Globals.Y3 = averageWidth;
                }
                Globals.isvalueAssigned = true;
            }
            else if (Globals.appState == AppState.secondCoordinate)
            {
                bool valuesCheck = ValuesCheck();
                if (valuesCheck)
                {
                    Globals.X4 = averageLength;
                    Globals.Y4 = averageWidth;
                }
                Globals.isvalueAssigned = true;
            }
            else if (Globals.appState == AppState.thirdCoordinate)
            {
                bool valuesCheck = ValuesCheck();
                if (valuesCheck)
                {
                    Globals.X2 = averageLength;
                    Globals.Y2 = averageWidth;
                }
                Globals.isvalueAssigned = true;
            }
            else if (Globals.appState == AppState.fourthCoordinate)
            {
                bool valuesCheck = ValuesCheck();
                if (valuesCheck)
                {
                    Globals.X1 = averageLength;
                    Globals.Y1 = averageWidth;
                }
                Globals.isvalueAssigned = true;
            }
            OnPropertyChanged(new PropertyChangedEventArgs("KinectSwipeDetected"));
        }

        public bool ValuesCheck()
        {
            //if(!Globals.isvalueAssigned)
            return true;
            //else 
            //    return false;
        }

        public KinectHelper()
        {
            try
            {

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


        void sensor_SkeletonFrameReady(object sender, BodyFrameArrivedEventArgs e)
        {
            Joint Spine = new Joint();
            Joint LeftFeet = new Joint();
            Joint RightFeet = new Joint();

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

                    frame.GetAndRefreshBodyData(this.bodies);

                    Body skeleton = GetPrimarySkeleton(this.bodies);

                    if (skeleton != null)
                    {
                        Spine = skeleton.Joints[JointType.SpineMid];
                        LeftFeet = skeleton.Joints[JointType.FootLeft];
                        RightFeet = skeleton.Joints[JointType.FootRight];

                        //Recognize Gestures either way
                        // update the gesture controller
                        gestureController.UpdateAllGestures(skeleton);

                        if (LeftFeet.TrackingState == TrackingState.Tracked && RightFeet.TrackingState == TrackingState.Tracked)
                        {
                            if (Globals.setCoordinates)
                            {
                                double averageWidth = (LeftFeet.Position.Z + RightFeet.Position.Z) / 2;
                                double averageLength = (LeftFeet.Position.X + RightFeet.Position.X) / 2;
                                SetCoordinates(averageWidth, averageLength);
                                Globals.setCoordinates = false;
                            }
                        }
                        else
                        {
                            CurrentLocation = -1;
                        }
                        //activeRecognizer.Recognize(sender, frame, this._FrameSkeletons);
                    }
                    else
                    {
                        CurrentLocation = -1;
                    }
                }

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
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            IReadOnlyDictionary<JointType, Joint> joints = skeletons[i].Joints;
                            IReadOnlyDictionary<JointType, Joint> jointsOld = skeleton.Joints;
                            if (jointsOld[JointType.SpineMid].Position.Z > joints[JointType.SpineMid].Position.Z)
                                skeleton = skeletons[i];
                        }
                    }
                }
            }
            return skeleton;
        }

        #endregion Event Handlers

    }
}
