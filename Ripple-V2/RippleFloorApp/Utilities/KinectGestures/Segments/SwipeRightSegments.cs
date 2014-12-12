using Microsoft.Kinect;

//Swipe left hand towards right of the body

namespace RippleFloorApp.Utilities.KinectGestures.Segments
{
    /// <summary>
    /// The first part of the swipe right gesture
    /// </summary>
    public class SwipeRightSegment1 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Body skeleton)
        {
            // //left hand in front of left Elbow and right hand is below right shoulder
            if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.SpineShoulder].Position.Y)
            {
                //left hand is above the average height of spinebase and hip
                if (skeleton.Joints[JointType.HandLeft].Position.Y > ((skeleton.Joints[JointType.SpineBase].Position.Y + skeleton.Joints[JointType.SpineMid].Position.Y) / 2))
                {
                    // //left hand left of left Shoulder by atleast 25 cm
                    if (skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.ShoulderLeft].Position.X < -0.25)
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

    /// <summary>
    /// The second part of the swipe right gesture
    /// </summary>
    public class SwipeRightSegment2 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Body skeleton)
        {
            // //left hand in front of left elbow and right hand is below shoulder
            if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.SpineShoulder].Position.Y)
            {
                //left hand is above the average height of spinebase and hip
                if (skeleton.Joints[JointType.HandLeft].Position.Y > ((skeleton.Joints[JointType.SpineBase].Position.Y + skeleton.Joints[JointType.SpineMid].Position.Y) / 2))
                {
                    // //left hand is left of the left shoulder 
                    if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X && skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X)
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

    /// <summary>
    /// The third part of the swipe right gesture
    /// </summary>
    public class SwipeRightSegment3 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Body skeleton)
        {
            // //left hand in front of left Shoulder
            if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y)
            {
                //left hand is above the average height of spinebase and hip
                if (skeleton.Joints[JointType.HandLeft].Position.Y > ((skeleton.Joints[JointType.SpineBase].Position.Y + skeleton.Joints[JointType.SpineMid].Position.Y) / 2))
                {
                    // //left hand left of right Hip
                    if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.HipRight].Position.X)
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
