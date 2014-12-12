using Microsoft.Kinect;

namespace RippleFloorApp.Utilities.KinectGestures.Segments.LegSwipe
{
    class LegSwipeSegment1 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {

            // Feets in the normal position
            if (skeleton.Joints[JointType.FootRight].Position.X - skeleton.Joints[JointType.FootLeft].Position.X > 0.015)
            {
                if (skeleton.Joints[JointType.FootRight].Position.Z - skeleton.Joints[JointType.FootLeft].Position.Z < - 0.05)
                {
                    return GesturePartResult.Succeed;
                }
                return GesturePartResult.Pausing;
            }
            return GesturePartResult.Fail;
        }
    }
}
