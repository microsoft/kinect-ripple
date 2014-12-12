using System;
using System.Collections.Generic;

namespace RippleDictionary
{
    public class Floor
    {
        #region Fields
        private double lockingPeriod;
        private String setupID;
        private int systemAutoLockPeriod;
        #endregion

        #region Constructors
        public Floor(Start start, Transition transition, Dictionary<string, Tile> tiles, double _lockingPeriod, int _systemAutoLockPeriod, String _setupID, Tile upperTile)
        {
            Start = start;
            Transition = transition;
            Tiles = tiles;
            LockingPeriod = _lockingPeriod;
            SystemAutoLockPeriod = _systemAutoLockPeriod;
            SetupID = _setupID;
            UpperTile = upperTile;
        }
        #endregion

        #region Properties
        public double LockingPeriod
        {
            get
            {
                return lockingPeriod;
            }
            set
            {
                lockingPeriod = value;
            }
        }

        public String SetupID
        {
            get
            {
                return setupID;
            }
            set
            {
                setupID  = value;
            }
        }

        public int SystemAutoLockPeriod
        {
            get
            {
                return systemAutoLockPeriod;
            }
            set
            {
                systemAutoLockPeriod = value;
            }
        }
        #endregion

        #region Objects
        public Start Start;
        public Transition Transition;
        public Dictionary<string, Tile> Tiles;
        public Tile UpperTile;
        #endregion
    }

    public class Transition
    {
        #region Constructors
        public Transition(string music, string animation)
        {
            Music = music;
            Animation = animation;
        }
        #endregion

        #region Properties
        public string Music
        {
            get;
            set;
        }

        public string Animation
        {
            get;
            set;
        }
        #endregion
    }

    //public class UpperTile
    //{
    //    #region Fields
    //    private string name;
    //    private string content;
    //    #endregion

    //    #region Constructors
    //    public UpperTile(string name, string content, Style style, Coordinate coordinate, bool loopVideo)
    //    {
    //        Name = name;
    //        Content = content;
    //        Style = style;
    //        Coordinate = coordinate;
    //        LoopVideo = loopVideo;
    //    }
    //    #endregion

    //    #region Properties
    //    public string Name
    //    {
    //        get
    //        {
    //            if (name == null)
    //            {
    //                throw new NullReferenceException();
    //            }
    //            else
    //            {
    //                return name;
    //            }
    //        }
    //        set
    //        {
    //            name = value;
    //        }
    //    }

    //    public string Content
    //    {
    //        get
    //        {
    //            if (content == null)
    //            {
    //                throw new NullReferenceException();
    //            }
    //            else
    //            {
    //                return content;
    //            }
    //        }
    //        set
    //        {
    //            content = value;
    //        }
    //    }

    //    public bool LoopVideo
    //    {
    //        get;
    //        set;
    //    }
    //    #endregion

    //    #region Attribute Objects
    //    public Style Style;
    //    public Coordinate Coordinate;
    //    #endregion
    //}
}
