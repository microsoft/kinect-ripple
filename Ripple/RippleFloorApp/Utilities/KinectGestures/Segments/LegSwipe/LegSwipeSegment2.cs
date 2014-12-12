using Microsoft.Kinect;

namespace RippleFloorApp.Utilities.KinectGestures.Segments.LegSwipe
{
    class LegSwipeSegment2 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {           
            if (skeleton.Joints[JointType.FootRight].Position.Z < skeleton.Joints[JointType.FootLeft].Position.Z)
            {
                if (skeleton.Joints[JointType.FootRight].Position.X < skeleton.Joints[JointType.FootLeft].Position.X)
                {
                    return GesturePartResult.Succeed;
                }
                return GesturePartResult.Pausing;
            }
            return GesturePartResult.Fail;
        }
    }
}
