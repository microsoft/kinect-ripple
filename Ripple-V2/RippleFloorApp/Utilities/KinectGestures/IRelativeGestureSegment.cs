using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace RippleFloorApp.Utilities.KinectGestures
{
    /// <summary>
    /// Defines a single gesture segment which uses relative positioning 
    /// of body parts to detect a gesture
    /// </summary>
    public interface IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        GesturePartResult CheckGesture(Body skeleton);
    }
}
