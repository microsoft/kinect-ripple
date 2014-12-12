using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace RippleCalibration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region AnimationProperties
        Storyboard blinkStep0;
        Storyboard blinkStep10;
        Storyboard blinkStep100;
        Storyboard blinkStep1000;

        Storyboard blinkStep1;
        Storyboard blinkStep2;
        Storyboard blinkStep3;
        Storyboard blinkStep4;
        Storyboard blinkStep5;
        Storyboard blinkStep6;
        Storyboard blinkStep7;
        Storyboard blinkStep8;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Utilities.KinectHelper kinectHelper = new Utilities.KinectHelper();
            kinectHelper.PropertyChanged += kinectHelper_PropertyChanged;
            Globals.ResetCoordinates();
            blinkStep0 = (Storyboard)this.MainGrid.FindResource("RectangleCyanBlink");
            blinkStep10 = (Storyboard)this.MainGrid.FindResource("RectangleGreenBlink");
            blinkStep100 = (Storyboard)this.MainGrid.FindResource("RectangleYellowBlink");
            blinkStep1000 = (Storyboard)this.MainGrid.FindResource("RectangleRedBlink");
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            blinkStep1 = blinkStep0.Clone();
            blinkStep2 = blinkStep0.Clone();
            blinkStep3 = blinkStep10.Clone();
            blinkStep4 = blinkStep10.Clone();
            blinkStep5 = blinkStep100.Clone();
            blinkStep6 = blinkStep100.Clone();
            blinkStep7 = blinkStep1000.Clone();
            blinkStep8 = blinkStep1000.Clone();

            Storyboard.SetTarget(blinkStep1.Children[0], this.Step1a);
            Storyboard.SetTarget(blinkStep2.Children[0], this.Step1b);
            Storyboard.SetTarget(blinkStep3.Children[0], this.Step2a);
            Storyboard.SetTarget(blinkStep4.Children[0], this.Step2b);
            Storyboard.SetTarget(blinkStep5.Children[0], this.Step3a);
            Storyboard.SetTarget(blinkStep6.Children[0], this.Step3b);
            Storyboard.SetTarget(blinkStep7.Children[0], this.Step4a);
            Storyboard.SetTarget(blinkStep8.Children[0], this.Step4b);

            blinkStep1.Begin();
            blinkStep2.Begin();

            A1Value.Visibility = Visibility.Collapsed;
            A2Value.Visibility = Visibility.Collapsed;
            A3Value.Visibility = Visibility.Collapsed;
            A4Value.Visibility = Visibility.Collapsed;
            B1Value.Visibility = Visibility.Collapsed;
            B2Value.Visibility = Visibility.Collapsed;
            B3Value.Visibility = Visibility.Collapsed;
            B4Value.Visibility = Visibility.Collapsed;

            Message.Text = "Stand on blue blinking footmark";

            Circle.Height = SystemParameters.PrimaryScreenWidth / 2.5;
            Circle.Width = SystemParameters.PrimaryScreenWidth / 2.5;
            //var h = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualHeight;
            //var w = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualWidth; 
        }

        void kinectHelper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var kinectInfo = sender as Utilities.KinectHelper;
            if (kinectInfo != null)
            {
                if (e.PropertyName == "KinectSwipeDetected")
                {
                    if (kinectInfo.KinectSwipeDetected.StartsWith("Right"))
                    {
                        OnHandSwipeDetected("Right");
                    }
                }
            }
        }


        private void OnHandSwipeDetected(string p)
        {
            if (Globals.appState == AppState.firstCoordinate)
            {
                Message.Text = "";
                //Check if the person is on left hand side and at distance more than 2 mts
                if (Globals.X3 < 0 && Globals.Y3 > 2.00)
                {
                    X3Value.Text = Convert.ToString(Globals.X3);
                    Y3Value.Text = Convert.ToString(Globals.Y3);
                    Globals.appState = AppState.secondCoordinate;
                    Message.Text = "Great! Go to green footmark";

                    blinkStep1.Stop();
                    blinkStep2.Stop();

                    blinkStep3.Begin();
                    blinkStep4.Begin();
                }
                else
                {
                    Message.Text = "Caution! You are standing at wrong Location";
                }
            }
            else if (Globals.appState == AppState.secondCoordinate)
            {
                Message.Text = "";
                //Check if the Y height of second point doesn't differ to 1st point by more than 20 cm
                if (Globals.X4 > 0 && Globals.Y4 > 2.00 && Math.Abs(Globals.Y4 - Globals.Y3) < 0.2)
                {
                    X4Value.Text = Convert.ToString(Globals.X4);
                    Y4Value.Text = Convert.ToString(Globals.Y4);
                    Globals.appState = AppState.thirdCoordinate;
                    Message.Text = "Great! Go to yellow footmark";

                    blinkStep3.Stop();
                    blinkStep4.Stop();

                    blinkStep5.Begin();
                    blinkStep6.Begin();
                }
                else
                {
                    Message.Text = "Caution! You are standing at wrong Location";
                }
            }
            else if (Globals.appState == AppState.thirdCoordinate)
            {
                Message.Text = "";
                if (Globals.X2 > 0 && Globals.Y2 > 0.8 && (Math.Abs(Globals.Y2 - Globals.Y4) > 1) && (Math.Abs(Globals.X2 - Globals.X4) < 0.2))
                {
                    X2Value.Text = Convert.ToString(Globals.X2);
                    Y2Value.Text = Convert.ToString(Globals.Y2);
                    Globals.appState = AppState.fourthCoordinate;
                    Message.Text = "Great! Go to red footmark";

                    blinkStep5.Stop();
                    blinkStep6.Stop();

                    blinkStep7.Begin();
                    blinkStep8.Begin();
                }
                else
                {
                    Message.Text = "Caution! You are standing at wrong Location";
                }
            }
            else if (Globals.appState == AppState.fourthCoordinate)
            {
                Message.Text = "";
                //Check if the person is on left side of the screen, more than .8 meters from kinect and the length of the screen is not lss than 1 mt
                if (Globals.X1 < 0 && Globals.Y1 > 0.8 && (Math.Abs(Globals.Y1 - Globals.Y3) > 1) && (Math.Abs(Globals.Y2 - Globals.Y1) < 0.2) && (Math.Abs(Globals.X1 - Globals.X3) < 0.2))
                {
                    X1Value.Text = Convert.ToString(Globals.X1);
                    Y1Value.Text = Convert.ToString(Globals.Y1);
                    Globals.appState = AppState.Done;
                    CalculateEndCoordinates();

                    blinkStep7.Stop();
                    blinkStep8.Stop();
                }
                else
                {
                    Message.Text = "Caution! You are standing at wrong Location";
                }
            }
            Globals.isvalueAssigned = false;

        }

        public void CalculateEndCoordinates()
        {
            Globals.A1 = CalculatePointA1();
            Globals.B1 = CalculatePointB1();
            Globals.A2 = CalculatePointA2();
            Globals.B2 = CalculatePointB2();
            Globals.A3 = CalculatePointA3();
            Globals.B3 = CalculatePointB3();
            Globals.A4 = CalculatePointA4();
            Globals.B4 = CalculatePointB4();

            Globals.frontDistance = CalculateFrontDistance();
            Globals.backDistance = CalculateBackDistance();
            Globals.leftDistance = CalculateLeftDistance();
            Globals.rightDistance = CalculateRightDistance();

            if (Math.Abs(Globals.rightDistance + Globals.leftDistance) < 0.2)
            {
                FrontValue.Text = Convert.ToString(Globals.frontDistance);
                BackValue.Text = Convert.ToString(Globals.backDistance);
                LeftValue.Text = Convert.ToString(Globals.leftDistance);
                RightValue.Text = Convert.ToString(Globals.rightDistance);

                CreateXML();

                ShowFinalValuesOnUI();
                Message.Text = "You are done! Have Fun!";
            }
            else
            {
                Message.Text = "Projection is Skewed. Make adjustment and start the calibration again";
            }
        }

        private void CreateXML()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Ripple");
            XmlElement floor = doc.CreateElement("Floor");
            XmlElement id = doc.CreateElement("CalibrationConfiguration");

            id.SetAttribute("FrontDistance", Convert.ToString(Globals.frontDistance));
            id.SetAttribute("BackDistance", Convert.ToString(Globals.backDistance));
            id.SetAttribute("LeftDistance", Convert.ToString(Globals.leftDistance));
            id.SetAttribute("RightDistance", Convert.ToString(Globals.rightDistance));
            id.SetAttribute("PrimaryScreenWidth", Convert.ToString(SystemParameters.PrimaryScreenWidth));
            id.SetAttribute("PrimaryScreenHeight", Convert.ToString(SystemParameters.PrimaryScreenHeight));

            floor.AppendChild(id);
            root.AppendChild(floor);
            doc.AppendChild(root);

            String calibXMLFilePath = GetCalibrationXMLPath();
            try
            {
                doc.Save(calibXMLFilePath);
            }
            catch (Exception ex)
            {
                Message.Text = ex.ToString();
            }
        }

        private string GetCalibrationXMLPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\..\\Calibration.xml";
        }

        private double CalculateLeftDistance()
        {
            double distance = 0.00;

            distance = (Globals.A1 + Globals.A3) / 2;
            return distance;
        }
        private double CalculateRightDistance()
        {
            double distance = 0.00;

            distance = (Globals.A2 + Globals.A4) / 2;
            return distance;
        }

        private double CalculateFrontDistance()
        {
            double distance = 0.00;

            distance = (Globals.B1 + Globals.B2) / 2;
            return distance;
        }


        private double CalculateBackDistance()
        {
            double distance = 0.00;
            distance = (Globals.B3 + Globals.B4) / 2;
            return distance;
        }

        public void ShowFinalValuesOnUI()
        {
            A1Value.Text = Convert.ToString(Globals.A1);
            B1Value.Text = Convert.ToString(Globals.B1);
            A2Value.Text = Convert.ToString(Globals.A2);
            B2Value.Text = Convert.ToString(Globals.B2);
            A3Value.Text = Convert.ToString(Globals.A3);
            B3Value.Text = Convert.ToString(Globals.B3);
            A4Value.Text = Convert.ToString(Globals.A4);
            B4Value.Text = Convert.ToString(Globals.B4);
        }

        public double CalculatePointA1()
        {
            double x = 0.00;
            x = (5 * Globals.X1 - Globals.X2) / 4;
            return x;
        }
        public double CalculatePointB1()
        {
            double y = 0.00;
            y = (17 * Globals.Y1 - 3 * Globals.Y3) / 14;
            return y;
        }
        public double CalculatePointA2()
        {
            double x = 0.00;
            x = (5 * Globals.X2 - Globals.X1) / 4;
            return x;
        }
        public double CalculatePointB2()
        {
            double y = 0.00;
            y = (17 * Globals.Y2 - 3 * Globals.Y4) / 14;
            return y;
        }
        public double CalculatePointA3()
        {
            double x = 0.00;
            x = (5 * Globals.X3 - Globals.X4) / 4;
            return x;
        }
        public double CalculatePointB3()
        {
            double y = 0.00;
            y = (17 * Globals.Y3 - 3 * Globals.Y1) / 14;
            return y;
        }
        public double CalculatePointA4()
        {
            double x = 0.00;
            x = (5 * Globals.X4 - Globals.X3) / 4;
            return x;
        }
        public double CalculatePointB4()
        {
            double y = 0.00;
            y = (17 * Globals.Y4 - 3 * Globals.Y2) / 14;
            return y;
        }
    }
}
