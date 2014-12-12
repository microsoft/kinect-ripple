using System;
namespace RippleDictionary
{
    public class Start
    {
        #region Constructors
        public Start(Animation animation, Unlock unlock, int introVideoWaitPeriod)
        {
            Animation = animation;
            Unlock = unlock;
            IntroVideoWaitPeriod = introVideoWaitPeriod;
        }
        #endregion

        #region Properties
        public int IntroVideoWaitPeriod
        {
            get;
            set;
        }
        #endregion

        #region Attribute objects
        public Animation Animation;

        public Unlock Unlock; 
        #endregion
    }

    public class Animation
    {
        #region Constructors
        public Animation(string name, string content, AnimationType animType)
        {
            Name = name;
            Content = content;
            AnimType = animType;
        } 
        #endregion

        #region Properties
        public string Name
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public AnimationType AnimType
        {
            get;
            set;
        }


        #endregion
    }

    public enum AnimationType
    {
        HTML
    }

    public class Unlock
    {
        #region Constructors
        public Unlock(Mode mode, String unlockType)
        {
            Mode = mode;
            UnlockType = unlockType;
        } 
        #endregion

        #region Properties
        public Mode Mode
        {
            get;
            set;
        }

        public String UnlockType
        {
            get;
            set;
        } 
        #endregion
    }

    public enum Mode
    {
        Gesture,
        HTML
    }

    public enum GestureUnlockType
    {
        LeftSwipe,
        RightSwipe,
        JoinedHands,
        SwipeUp
    }    
}
