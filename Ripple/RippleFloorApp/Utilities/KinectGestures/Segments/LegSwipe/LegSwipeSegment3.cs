using Microsoft.Kinect;

namespace RippleFloorApp.Utilities.KinectGestures.Segments.LegSwipe
{
    class LegSwipeSegment3 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            
             if (skeleton.Joints[JointType.FootRight].Position.X - skeleton.Joints[JointType.FootLeft].Position.X < -0.015)
                {
                    if (skeleton.Joints[JointType.FootRight].Position.Z - skeleton.Joints[JointType.FootLeft].Position.Z > -0.1)
                    {
                    return GesturePartResult.Succeed;
                    }
                    return GesturePartResult.Pausing;
                }                        
            return GesturePartResult.Fail;
        }
    }
}
