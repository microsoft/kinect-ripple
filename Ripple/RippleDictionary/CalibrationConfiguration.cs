using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleDictionary
{
    public class CalibrationConfiguration
    {
        #region Fields
        private double frontDistance, backDistance, leftDistance, rightDistance, primaryScreenWidth, primaryScreenHeight;
        #endregion

        #region Constructors
        public CalibrationConfiguration(string t_frontDistance, string t_backDistance, string t_leftDistance, string t_rightDistance, string t_primaryScreenWidth, string t_primaryScreenHeight)
        {
            FrontDistance = Convert.ToDouble(t_frontDistance);
            BackDistance = Convert.ToDouble(t_backDistance);
            LeftDistance = Convert.ToDouble(t_leftDistance);
            RightDistance = Convert.ToDouble(t_rightDistance);
            PrimaryScreenWidth = Convert.ToDouble(t_primaryScreenWidth);
            PrimaryScreenHeight = Convert.ToDouble(t_primaryScreenHeight);
        }
        #endregion

        #region Properties
        public double FrontDistance
        {
            get
            {
                return frontDistance;
            }
            set
            {
                frontDistance = value;
            }
        }

        public double BackDistance
        {
            get
            {
                return backDistance;
            }
            set
            {
                backDistance = value;
            }
        }

        public double LeftDistance
        {
            get
            {
                return leftDistance;
            }
            set
            {
                leftDistance = value;
            }
        }

        public double RightDistance
        {
            get
            {
                return rightDistance;
            }
            set
            {
                rightDistance = value;
            }
        }

        public double PrimaryScreenWidth
        {
            get
            {
                return primaryScreenWidth;
            }
            set
            {
                primaryScreenWidth = value;
            }
        }

        public double PrimaryScreenHeight
        {
            get
            {
                return primaryScreenHeight;
            }
            set
            {
                primaryScreenHeight = value;
            }
        }
        #endregion
    }
}
