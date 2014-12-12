using Microsoft.Kinect;

namespace RippleFloorApp.Utilities.KinectGestures.Segments
{
    /// <summary>
    /// The third part of the swipe up gesture
    /// </summary>
    public class SwipeUpSegment3 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // //Right hand in front of right shoulder
            if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z && (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ShoulderLeft].Position.Z))
            {
                // right hand above head
                if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X && (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X))
                {
                    // right hand right of right shoulder
                    if ((skeleton.Joints[JointType.HandRight].Position.Y - (skeleton.Joints[JointType.Head].Position.Y)) > 0.10 && ((skeleton.Joints[JointType.HandLeft].Position.Y - (skeleton.Joints[JointType.Head].Position.Y)) > 0.10))
                    {
                        return GesturePartResult.Succeed;
                    }
                    return GesturePartResult.Pausing;
                }

                return GesturePartResult.Fail;
            }
            return GesturePartResult.Fail;
        }
    }
}
