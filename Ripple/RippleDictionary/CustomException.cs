using System;

namespace RippleDictionary
{
    public class TypeNotKnownException : Exception
    {
        #region Private Fields
        string type = null;
        #endregion

        #region Constructors
        public TypeNotKnownException()
        {
        }

        public TypeNotKnownException(string type)
        {
            this.type = type;
        } 
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("Type - '{0}' Not Known.", type);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class UnlockTypeNotKnownException : Exception
    {
        #region Private Fields
        string unlocktype = null;
        #endregion

        #region Constructors
        public UnlockTypeNotKnownException()
        {
        }

        public UnlockTypeNotKnownException(string unlockType)
        {
            this.unlocktype = unlockType;
        } 
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("UnlockType - '{0}' Not Known.", unlocktype);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class ModeNotKnownException : Exception
    {
        #region Private Fields
        string mode = null;
        #endregion

        #region Constructors
        public ModeNotKnownException()
        {
        }

        public ModeNotKnownException(string mode)
        {
            this.mode = mode;
        } 
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("Mode - '{0}' Not Known.", mode);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class AnimationTypeNotKnownException : Exception
    {
        #region Private Fields
        string animType = null;
        #endregion

        #region Constructors
        public AnimationTypeNotKnownException()
        {
        }

        public AnimationTypeNotKnownException(string animType)
        {
            this.animType = animType;
        }
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("Mode - '{0}' Not Known.", animType);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class TileTypeNotKnownException : Exception
    {
        #region Private Fields
        string tileType = null;
        #endregion

        #region Constructors
        public TileTypeNotKnownException()
        {
        }

        public TileTypeNotKnownException(string tileType)
        {
            this.tileType = tileType;
        } 
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("TileType - '{0}' Not Known.", tileType);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class InvalidCoordinateException : Exception
    {
        #region Private Fields
        string coordinates = null;
        #endregion

        #region Constructors
        public InvalidCoordinateException()
        {
        }

        public InvalidCoordinateException(string coordinates)
        {
            this.coordinates = coordinates;
        } 
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("Coordinates - '{0}' is not valid.", coordinates);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class InvalidStyleException : Exception
    {
        #region Private Fields
        string style = null;
        #endregion

        #region Constructors
        public InvalidStyleException()
        {
        }

        public InvalidStyleException(string style)
        {
            this.style = style;
        } 
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("Invalid style - {0}. Height and width expected.", style);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class UndefinedUnlockException : Exception
    {
        #region Private Fields
        string unlock = null;
        #endregion

        #region Constructors
        public UndefinedUnlockException()
        {
        }

        public UndefinedUnlockException(string unlock)
        {
            this.unlock = unlock;
        } 
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("Unlock - '{0}' Not Known.", unlock);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class UnparseableXMLException : Exception
    {
        #region Overriden Properties
        public override string Message
        {
            get
            {
                return "The XML cannot be parsed. Check the XML format.";
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }

    public class TileActionNotKnownException : Exception
    {
        #region Private Fields
        string action = null;
        #endregion

        #region Constructors
        public TileActionNotKnownException()
        {
        }

        public TileActionNotKnownException(string action)
        {
            this.action = action;
        } 
        #endregion

        #region Overriden Properties
        public override string Message
        {
            get
            {
                return string.Format("Tile Action - '{0}' Not Known.", action);
            }
        }

        public override string HelpLink
        {
            get
            {
                return "http://aka.ms/ripple";
            }
        }
        #endregion
    }
}
