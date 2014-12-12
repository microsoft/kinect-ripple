using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace RippleDictionary
{
    public class Tile
    {
        #region Fields
        private string id;
        private string name;
        private Color color;
        #endregion

        #region Constructors
        public Tile(string id, string name, TileType tileType, string color, Style style, Coordinate coordinate, TileAction action, string actionURI, string content = null, ContentType? correspondingScreenContentType = null, Dictionary<string, Tile> subTiles = null)
        {
            Id = id;
            Name = name;
            TileType = tileType;
            Color = (Color)ColorConverter.ConvertFromString(color);
            Style = style;
            Coordinate = coordinate;
            Action = action;
            ActionURI = actionURI;
            Content = content;
            CorrespondingScreenContentType = correspondingScreenContentType;
            SubTiles = subTiles;
        }
        #endregion

        #region Properties
        public string Id
        {
            get
            {
                if (id == null)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return id;
                }
            }
            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                if (name == null)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return name;
                }
            }
            set
            {
                name = value;
            }
        }

        public TileType TileType
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public ContentType? CorrespondingScreenContentType
        {
            get;
            set;
        }

        public Color Color
        {
            // To do
            // Code for Color maps

            get
            {
                if (color == null)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return color;
                }
            }
            set
            {
                color = value;
            }
        }

        public TileAction Action
        {
            get;
            set;
        }

        public string ActionURI
        {
            get;
            set;
        }
        #endregion

        #region Attribute Objects
        public Style Style;
        public Coordinate Coordinate;
        public Dictionary<string, Tile> SubTiles;
        #endregion

    }

    public class Style
    {
        #region Fields
        private double width, height;
        #endregion

        #region Constructors
        public Style(double width, double height)
        {
            Width = width;
            Height = height;
        }
        #endregion

        #region Properties
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }
        #endregion
    }

    public class Coordinate
    {
        #region Fields
        private double x, y;
        #endregion

        #region Constructors
        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Properties
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        #endregion
    }

    public enum TileAction
    {
        Standard,
        Nothing,
        NothingOnFloor,
        HTML,
        QRCode,
        Logout
    }
}
