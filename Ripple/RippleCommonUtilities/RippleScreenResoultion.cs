using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleCommonUtilities
{
    public class RippleScreenResoultion
    {
        private String screenName;
        private double horizontalResoultion;
        private double verticalResolution;
        private double xOrigin;
        private double yOrigin;

        public String ScreenName
        {
            get { return screenName; }
            set { screenName = value; }
        }

        public double XOrigin
        {
            get { return xOrigin; }
            set { xOrigin = value; }
        }

        public double YOrigin
        {
            get { return yOrigin; }
            set { yOrigin = value; }
        }

        public double HorizontalResolution
        {
            get { return horizontalResoultion; }
            set { horizontalResoultion = value; }
        }

        public double VerticalResolution
        {
            get { return verticalResolution; }
            set { verticalResolution = value; }
        }
        
    }
}
