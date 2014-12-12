using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Timers;

namespace RippleCalibration.Utilities
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
        private String kinectConnectionState;
        private String kinectSwipeDetected;
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
                if (kinectSwipeDetected.Contains("Right"))
                {
                    Globals.setCoordinates = true;
                }
                else if (kinectSwipeDetected.Contains("Left"))
                {
                    // Globals.ResetCoordinates();
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

        void Initialize()
        {
            if (sensor != null)
            {
                this.sensor.SkeletonStream.Enable();
                this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
                this._FrameSkeletons = new Skeleton[this.sensor.SkeletonStream.FrameSkeletonArrayLength];
                sensor.Start();
                KinectConnectionState = "Connected";

                // add timer for clearing last detected gesture
                _clearTimer = new Timer(2000);
                _clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);
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

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Joint Spine = new Joint();
            Joint LeftFeet = new Joint();
            Joint RightFeet = new Joint();

            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this._FrameSkeletons);

                    Skeleton skeleton = GetPrimarySkeleton(this._FrameSkeletons);

                    if (skeleton != null)
                    {
                        Spine = skeleton.Joints[JointType.Spine];
                        LeftFeet = skeleton.Joints[JointType.FootLeft];
                        RightFeet = skeleton.Joints[JointType.FootRight];

                        if (Spine.TrackingState == JointTrackingState.Tracked)
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
                        activeRecognizer.Recognize(sender, frame, this._FrameSkeletons);
                    }
                    else
                    {
                        CurrentLocation = -1;
                    }
                }

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
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                                skeleton = skeletons[i];
                        }
                    }
                }
            }
            return skeleton;
        }

        /// <summary>
        /// Clear text after some time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Gesture = "";
            _clearTimer.Stop();
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
                KinectSwipeDetected = "Right Swipe";
            };

            // Wire-up swipe left to manually reverse picture.
            recognizer.SwipeLeftDetected += (s, e) =>
            {
                KinectSwipeDetected = "Left Swipe";
            };

            return recognizer;
        }


    }
}
