using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace RippleDictionary
{
    public static class Dictionary
    {
        #region Public Methods
        /// <summary>
        /// Returns the Ripple dictionary from the Ripple XML File path provided.
        /// </summary>
        /// <returns>RippleDictionary.Ripple</returns>
        /// <exception cref="System.NullReferenceException
        ///     System.ArgumentNullException
        ///     RippleDictionary.TypeNotKnownException
        ///     System.FormatException
        ///     System.OverflowException
        ///     RippleDictionary.UnparseableXMLException
        ///     RippleDictionary.UndefinedUnlockException
        ///     RippleDictionary.TileTypeNotKnownException
        ///     RippleDictionary.InvalidStyleException
        ///     RippleDictionary.InvalidCoordinateException
        ///     RippleDictionary.ActionNotKnownException
        ///     RippleDictionary.TypeNotKnownException" />
        public static Ripple GetRippleDictionaryFromFile(string rippleFile = null)
        {
            StreamReader file = new StreamReader(rippleFile == null ? XMLElementsAndAttributes.RippleXMLFile : rippleFile);

            string xml = file.ReadToEnd();

            Floor floor = GetFloorFromXML(xml);
            Screen screen = GetScreenFromXML(xml);

            Ripple ripple = new Ripple(screen, floor);

            return ripple;
        }

        public static Ripple GetRipple(String filePath)
        {
            StreamReader file = new StreamReader(filePath + "\\..\\" + XMLElementsAndAttributes.RippleXMLFile);

            string xml = file.ReadToEnd();

            Floor floor = GetFloorFromXML(xml);
            Screen screen = GetScreenFromXML(xml);

            Ripple ripple = new Ripple(screen, floor);

            return ripple;
        }

        /// <summary>
        /// Get the Floor tag from the file Calibration.xml existing in the execution directory.
        /// </summary>
        /// <returns>FloorCalibration.Floor</returns>
        /// <exception cref="System.NullReferenceException
        ///     System.ArgumentException
        ///     System.ArgumentNullException
        ///     System.FormatException
        ///     System.IO.FileNotFoundException
        ///     System.OverflowException
        ///     FloorCalibration.UnparseableXMLException" />
        public static CalibrationConfiguration GetFloorConfigurations(String filePath)
        {
            StreamReader file = new StreamReader(filePath + "\\..\\" + XMLElementsAndAttributes.CalibrationXML);
            string xml = file.ReadToEnd();

            return GetFloorConfigFromXML(xml);
        }

        /// <summary>
        /// Gets the Screen tag from the file RippleXML.xml existing in the execution directory.
        /// </summary>
        /// <returns>RippleDictionary.Screen</returns>
        /// <exception cref="System.NullReferenceException
        ///     System.ArgumentNullException
        ///     RippleDictionary.TypeNotKnownException
        ///     RippleDictionary.UnparseableXMLException" />
        public static Screen GetScreen(String filePath)
        {
            Screen screen = null;
            StreamReader file = new StreamReader(filePath + "\\..\\" + XMLElementsAndAttributes.RippleXMLFile);

            string xml = file.ReadToEnd();

            screen = GetScreenFromXML(xml);

            return screen;
        }

        /// <summary>
        /// Gets the Screen tag from the Ripple XML.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>RippleDictionary.Screen</returns>
        /// <exception cref="System.NullReferenceException
        ///     System.ArgumentNullException
        ///     RippleDictionary.TypeNotKnownException
        ///     RippleDictionary.UnparseableXMLException" />
        public static Screen GetScreenFromXML(string xml)
        {
            Screen screen = null;
            XDocument xdoc = XDocument.Load(GenerateRippleDictionaryStreamFromXML(xml));

            foreach (var xel in xdoc.Root.Elements())
            {
                if (xel.Name == XMLElementsAndAttributes.Screen)
                {
                    
                    string type, id, header, content;
                    Dictionary<string, ScreenContent> screenContents = new Dictionary<string, ScreenContent>();
                    ScreenContent screenContent;
                    foreach (var tagContent in xel.Elements())
                    {
                        if (tagContent.Name == XMLElementsAndAttributes.ScreenContent)
                        {
                            type = tagContent.Attribute(XMLElementsAndAttributes.Type).Value;
                            id = tagContent.Attribute(XMLElementsAndAttributes.Id).Value;
                            header = tagContent.Attribute(XMLElementsAndAttributes.Header).Value;
                            content = tagContent.Attribute(XMLElementsAndAttributes.Content).Value;
                            var loopVideoObj = tagContent.Attribute(XMLElementsAndAttributes.LoopVideo);
                            bool loopVideo = loopVideoObj != null ? (loopVideoObj.Value.ToLower() == "true" ? (bool)true : (loopVideoObj.Value.ToLower() == "false" ? (bool)false : false)) : false;

                            screenContent = new ScreenContent(GetType(type), id, header, content, loopVideo);
                            screenContents.Add(screenContent.Id, screenContent);
                        }
                    }

                    screen = new Screen(screenContents); 
                    
                }
                else
                {
                    continue;
                }
            }

            return screen;
        }

        /// <summary>
        /// Gets the Floor tag from the file RippleXML.xml existing in the execution directory.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>RippleDictionary.Floor</returns>
        /// <exception cref="System.NullReferenceException
        ///     System.ArgumentNullException
        ///     System.FormatException
        ///     System.OverflowException
        ///     RippleDictionary.UndefinedUnlockException
        ///     RippleDictionary.TileTypeNotKnownException
        ///     RippleDictionary.InvalidStyleException
        ///     RippleDictionary.InvalidCoordinateException
        ///     RippleDictionary.TypeNotKnownException
        ///     RippleDictionary.ActionNotKnownException
        ///     RippleDictionary.UnparseableXMLException" />
        public static Floor GetFloor(String filePath)
        {
            Floor floor = null;
            StreamReader file = new StreamReader(filePath + "\\..\\" + XMLElementsAndAttributes.RippleXMLFile);

            string xml = file.ReadToEnd();

            floor = GetFloorFromXML(xml);

            return floor;
        }

        /// <summary>
        /// Gets the Floor tag from the Ripple XML.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>RippleDictionary.Floor</returns>
        /// <exception cref="System.NullReferenceException
        ///     System.ArgumentNullException
        ///     System.FormatException
        ///     System.OverflowException
        ///     RippleDictionary.UndefinedUnlockException
        ///     RippleDictionary.TileTypeNotKnownException
        ///     RippleDictionary.InvalidStyleException
        ///     RippleDictionary.InvalidCoordinateException
        ///     RippleDictionary.TypeNotKnownException
        ///     RippleDictionary.ActionNotKnownException
        ///     RippleDictionary.UnparseableXMLException" />
        public static Floor GetFloorFromXML(string xml)
        {
            Floor floor = null;

            XDocument xdoc = XDocument.Load(GenerateRippleDictionaryStreamFromXML(xml));

            foreach (var xel in xdoc.Root.Elements())
            {
                if (xel.Name == XMLElementsAndAttributes.Floor)
                {
                    Start start = null;
                    Transition transition = null;
                    Tile upperTile = null;
                    Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();
                    double lockingPeriod = 0.0;
                    int systemAutoLockPeriod = 0;
                    String setupID = String.Empty;

                    foreach (var tagContent in xel.Elements())
                    {
                        if (tagContent.Name == XMLElementsAndAttributes.Start)
                        {
                            Animation animation;
                            Unlock unlock;
                            int introVideoWaitPeriod;

                            GetStartContent(tagContent, out animation, out unlock, out introVideoWaitPeriod);

                            start = new Start(animation, unlock, introVideoWaitPeriod);
                        }
                        else if (tagContent.Name == XMLElementsAndAttributes.Transition)
                        {
                            string music = tagContent.Attribute(XMLElementsAndAttributes.Music).Value;
                            string animation = tagContent.Attribute(XMLElementsAndAttributes.Animation).Value;

                            transition = new Transition(music, animation);
                        }
                        else if (tagContent.Name == XMLElementsAndAttributes.Tiles)
                        {
                            tiles = GetTilesDictionaryFromTag(tagContent);
                        }
                        else if (tagContent.Name == XMLElementsAndAttributes.UpperTile)
                        {
                            string id = tagContent.Attribute(XMLElementsAndAttributes.Id).Value;
                            string name = tagContent.Attribute(XMLElementsAndAttributes.Name).Value;
                            string tileType = tagContent.Attribute(XMLElementsAndAttributes.TileType).Value;
                            string content = tagContent.Attribute(XMLElementsAndAttributes.Content).Value;
                            string color = tagContent.Attribute(XMLElementsAndAttributes.Color).Value;
                            string action = tagContent.Attribute(XMLElementsAndAttributes.Action).Value;
                            var actionURIObj = tagContent.Attribute(XMLElementsAndAttributes.ActionURI);
                            string actionURI = actionURIObj != null ? actionURIObj.Value : null;
                            string style = tagContent.Attribute(XMLElementsAndAttributes.Style).Value;
                            string coordinate = tagContent.Attribute(XMLElementsAndAttributes.Coordinate).Value;
                            var correspondingScreenContentTypeObj = tagContent.Attribute(XMLElementsAndAttributes.CorrespondingScreenContentType);
                            string correspondingScreenContentType = correspondingScreenContentTypeObj != null ? correspondingScreenContentTypeObj.Value : null;

                            upperTile = new Tile(id, name, GetTileType(tileType), color, GetStyle(style), GetCoordinate(coordinate), GetAction(action), actionURI, content, GetType(correspondingScreenContentType), null);
                        }
                        else if (tagContent.Name == XMLElementsAndAttributes.LockingPeriod)
	                    {
		                    lockingPeriod = Convert.ToDouble(tagContent.Value);
	                    }
                        else if (tagContent.Name == XMLElementsAndAttributes.SystemAutoLockPeriod)
                        {
                            systemAutoLockPeriod = Convert.ToInt32(tagContent.Value);
                        }
                        else if (tagContent.Name == XMLElementsAndAttributes.SetupID)
                        {
                            setupID = tagContent.Value;
                        }
                    }

                    floor = new Floor(start, transition, tiles, lockingPeriod, systemAutoLockPeriod, setupID, upperTile);
                }
                else
                {
                    continue;
                }
            }

            return floor;
        }

        /// <summary>
        /// Gets all the tiles from from the file RippleXML.xml existing in the execution directory.
        /// </summary>
        /// <param name="tagContent"></param>
        /// <param name="parentTile"></param>
        /// <returns>"Dictionary&lt;string, RippleDictionary.Tile&gt;"</returns>
        /// <exception cref="System.ArgumentNullException
        ///     RippleDictionary.UnparseableXMLException
        ///     RippleDictionary.TileTypeNotKnownException
        ///     RippleDictionary.InvalidStyleException
        ///     RippleDictionary.InvalidCoordinateException
        ///     RippleDictionary.TypeNotKnownException
        ///     RippleDictionary.ActionNotKnownException" />
        public static Dictionary<string, Tile> GetTilesDictionary()
        {
            Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

            XDocument xdoc = XDocument.Load(GenerateRippleDictionaryStreamFromXML(XMLElementsAndAttributes.RippleXMLFile));

            foreach (var xel in xdoc.Root.Elements())
            {
                if (xel.Name == XMLElementsAndAttributes.Floor)
                {
                    foreach (var tagContent in xel.Elements())
                    {
                        if (tagContent.Name == XMLElementsAndAttributes.Tiles)
                        {
                            tiles = GetTilesDictionaryFromTag(tagContent);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            return tiles;
        }        
        #endregion

        #region Helpers

        /// <summary>
        /// Get the Floor tag from Calibration XML.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>FloorCalibration.Floor</returns>
        /// <exception cref="System.NullReferenceException
        ///     System.ArgumentNullException
        ///     System.FormatException
        ///     System.OverflowException
        ///     FloorCalibration.UnparseableXMLException" />
        private static CalibrationConfiguration GetFloorConfigFromXML(string xml)
        {
            XDocument xdoc = XDocument.Load(GenerateRippleDictionaryStreamFromXML(xml));
            CalibrationConfiguration configuration = null;

            foreach (var xel in xdoc.Root.Elements())
            {
                if (xel.Name == XMLElementsAndAttributes.Floor)
                {

                    foreach (var tagContent in xel.Elements())
                    {
                        if (tagContent.Name == XMLElementsAndAttributes.CalibrationConfiguration)
                        {
                            string frontDistance = tagContent.Attribute(XMLElementsAndAttributes.FrontDistance).Value;
                            string backDistance = tagContent.Attribute(XMLElementsAndAttributes.BackDistance).Value;
                            string leftDistance = tagContent.Attribute(XMLElementsAndAttributes.LeftDistance).Value;
                            string rightDistance = tagContent.Attribute(XMLElementsAndAttributes.RightDistance).Value;
                            string primaryScreenWidth = tagContent.Attribute(XMLElementsAndAttributes.PrimaryScreenWidth).Value;
                            string primaryScreenHeight = tagContent.Attribute(XMLElementsAndAttributes.PrimaryScreenHeight).Value;

                            configuration = new CalibrationConfiguration(frontDistance, backDistance, leftDistance, rightDistance, primaryScreenWidth, primaryScreenHeight);
                        }
                    }

                }
            }

            return configuration;
        }

        /// <summary>
        /// Gets all the tiles from the tagContent's parent node.
        /// </summary>
        /// <param name="tagContent"></param>
        /// <param name="parentTile"></param>
        /// <returns>"Dictionary&lt;string, RippleDictionary.Tile&gt;"</returns>
        /// <exception cref="System.ArgumentNullException
        ///     RippleDictionary.UnparseableXMLException
        ///     RippleDictionary.TileTypeNotKnownException
        ///     RippleDictionary.InvalidStyleException
        ///     RippleDictionary.InvalidCoordinateException
        ///     RippleDictionary.TypeNotKnownException
        ///     RippleDictionary.ActionNotKnownException" />
        private static Dictionary<string, Tile> GetTilesDictionaryFromTag(XElement tagContent)
        {
            Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

            foreach (var tileTag in tagContent.Elements())
            {
                Tile tile = GetTile(tileTag);

                tiles.Add(tile.Id, tile);
            }

            return tiles;
        }

        /// <summary>
        /// Gets the tile information from the tileTag.
        /// </summary>
        /// <param name="tileTag"></param>
        /// <returns>RippleDictionary.Tile</returns>
        /// <exception cref="System.ArgumentNullException
        ///     RippleDictionary.TileTypeNotKnownException
        ///     RippleDictionary.InvalidStyleException
        ///     RippleDictionary.InvalidCoordinateException
        ///     RippleDictionary.TypeNotKnownException
        ///     RippleDictionary.ActionNotKnownException" />
        private static Tile GetTile(XElement tileTag)
        {
            Tile tile = null;
            string id, name, tileType, content, color, action, actionURI, style, coordinate, correspondingScreenContentType;
            Dictionary<string, Tile> subTiles = new Dictionary<string, Tile>();

            id = tileTag.Attribute(XMLElementsAndAttributes.Id).Value;
            name = tileTag.Attribute(XMLElementsAndAttributes.Name).Value;
            tileType = tileTag.Attribute(XMLElementsAndAttributes.TileType).Value;
            content = tileTag.Attribute(XMLElementsAndAttributes.Content).Value;
            color = tileTag.Attribute(XMLElementsAndAttributes.Color).Value;
            action = tileTag.Attribute(XMLElementsAndAttributes.Action).Value;
            var actionURIObj = tileTag.Attribute(XMLElementsAndAttributes.ActionURI);
            actionURI = actionURIObj != null ? actionURIObj.Value : null;
            style = tileTag.Attribute(XMLElementsAndAttributes.Style).Value;
            coordinate = tileTag.Attribute(XMLElementsAndAttributes.Coordinate).Value;
            var correspondingScreenContentTypeObj = tileTag.Attribute(XMLElementsAndAttributes.CorrespondingScreenContentType);
            correspondingScreenContentType = correspondingScreenContentTypeObj != null ? correspondingScreenContentTypeObj.Value : null;

            if (tileTag.Elements().Count() != 0)
            {
                subTiles = GetTilesDictionaryFromTag(tileTag);
            }
            else
            {
                subTiles = null;
            }

            tile = new Tile(id, name, GetTileType(tileType), color, GetStyle(style), GetCoordinate(coordinate), GetAction(action), actionURI, content, GetType(correspondingScreenContentType), subTiles);

            return tile;
        }

        /// <summary>
        /// Gets the Start tag from the Ripple XML.
        /// </summary>
        /// <param name="tagContent"></param>
        /// <param name="animation"></param>
        /// <param name="unlock"></param>
        /// <param name="introVideoWaitPeriod"></param>
        /// <exception cref="System.FormatException
        ///     System.OverflowException
        ///     System.ArgumentNullException
        ///     RippleDictionary.UndefinedUnlockException" />
        private static void GetStartContent(XElement tagContent, out Animation animation, out Unlock unlock, out int introVideoWaitPeriod)
        {
            animation = null;
            unlock = null;
            introVideoWaitPeriod = 30; //Default

            foreach (var startContent in tagContent.Elements())
            {
                if (startContent.Name == XMLElementsAndAttributes.Animation)
                {
                    string animationName = startContent.Attribute(XMLElementsAndAttributes.Name).Value;
                    string animationContent = startContent.Attribute(XMLElementsAndAttributes.Content).Value;
                    string animationType = startContent.Attribute(XMLElementsAndAttributes.AnimationType).Value;

                    animation = new Animation(animationName, animationContent, GetAnimationType(animationType));
                }
                else if (startContent.Name == XMLElementsAndAttributes.Unlock)
                {
                    string mode = startContent.Attribute(XMLElementsAndAttributes.Mode).Value;
                    string unlockType = startContent.Attribute(XMLElementsAndAttributes.UnlockType).Value;

                    unlock = new Unlock(GetMode(mode), unlockType);
                }
                else if (startContent.Name == XMLElementsAndAttributes.IntroVideoWaitPeriod)
                {
                    string waitPeriod = startContent.Attribute(XMLElementsAndAttributes.Value).Value;

                    introVideoWaitPeriod = Convert.ToInt32(waitPeriod);
                }
            }

            if (unlock == null)
            {
                throw new UndefinedUnlockException();
            }
        }        

        /// <summary>
        /// Maps the string mode to the enum RippleDictionary.Mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>RippleDictionary.Mode</returns>
        /// <exception cref="RippleDictionary.ModeNotKnownException" />
        private static Mode GetMode(string mode)
        {
            switch (mode)
            {
                case "Gesture":
                    return Mode.Gesture;
                case "HTML":
                    return Mode.HTML;
                default:
                    throw new ModeNotKnownException(mode);
            }
        }

        /// <summary>
        /// Maps the string animation type value to the respective enum RippleDictionary.AnimationType.
        /// </summary>
        /// <param name="animtype"></param>
        /// <returns>RippleDictionary.AnimationType</returns>
        /// <exception cref="RippleDictionary.AnimationTypeNotKnownException" />
        private static AnimationType GetAnimationType(string animtype)
        {
            switch (animtype)
            {
                case "Flash":
                    return AnimationType.Flash;
                case "HTML":
                    return AnimationType.HTML;
                default:
                    throw new AnimationTypeNotKnownException(animtype);
            }
        }

        /// <summary>
        /// Maps the string coordinate to X &amp; Y coordinates.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns>RippleDictionary.Coordinate</returns>
        /// <exception cref="RippleDictionary.InvalidCoordinateException" />
        private static Coordinate GetCoordinate(string coordinate)
        {
            string[] xy = Regex.Split(coordinate, "; ");
            string error = "Error:Error";

            string x = Regex.Split((xy[0].Contains("X") ? xy[0] : (xy[1].Contains("X") ? xy[1] : error)), ":")[1].Trim(new char[] { ';', ' ' });
            string y = Regex.Split((xy[0].Contains("Y") ? xy[0] : (xy[1].Contains("Y") ? xy[1] : error)), ":")[1].Trim(new char[] { ';', ' ' });

            if (x == "Error" || y == "Error")
            {
                throw new InvalidCoordinateException();
            }

            x = x.Substring(1, x.Length - 2).Trim();
            y = y.Substring(1, y.Length - 2).Trim();

            try
            {
                return new Coordinate(Convert.ToDouble(x), Convert.ToDouble(y));
            }
            catch (Exception)
            {
                throw new InvalidCoordinateException(coordinate);
            }
        }

        /// <summary>
        /// Maps the style to height and width of the tile.
        /// </summary>
        /// <param name="style"></param>
        /// <returns>RippleDictionary.Style</returns>
        /// <exception cref="RippleDictionary.InvalidStyleException" />
        private static Style GetStyle(string style)
        {
            string[] widthAndHeight = Regex.Split(style, "; ");
            string error = "Error:Error";

            string width = Regex.Split((widthAndHeight[0].Contains("Width") ? widthAndHeight[0] : (widthAndHeight[1].Contains("Width") ? widthAndHeight[1] : error)), ":")[1].Trim(new char[] { ';', ' ' });
            string height = Regex.Split((widthAndHeight[0].Contains("Height") ? widthAndHeight[0] : (widthAndHeight[1].Contains("Height") ? widthAndHeight[1] : error)), ":")[1].Trim(new char[] { ';', ' ' });

            if (width == "Error" || height == "Error")
            {
                throw new InvalidStyleException(style);
            }

            width = width.Substring(1, width.Length - 2).Trim();
            height = height.Substring(1, height.Length - 2).Trim();

            try
            {
                return new Style(Convert.ToDouble(width), Convert.ToDouble(height));
            }
            catch (Exception)
            {
                throw new InvalidStyleException(style);
            }
        }

        /// <summary>
        /// Maps the string tileType to RippleDictionary.TileType.
        /// </summary>
        /// <param name="tileType"></param>
        /// <returns>RippleDictionary.TileType</returns>
        /// <exception cref="RippleDictionary.TileTypeNotKnownException" />
        private static TileType GetTileType(string tileType)
        {
            switch (tileType)
            {
                case "OnlyMedia":
                    return TileType.OnlyMedia;
                case "Text":
                    return TileType.Text;
                case "TextThumbnail":
                    return TileType.TextThumbnail;
                case "LiveTile":
                    return TileType.LiveTile;
                default:
                    throw new TileTypeNotKnownException(tileType);
            }
        }

        /// <summary>
        /// Maps the string type to the content type RippleDictionary.Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>RippleDictionary.Type</returns>
        /// <exception cref="RippleDictionary.ContentTypeNotKnownException" />
        private static ContentType GetType(string type)
        {
            switch (type)
            {
                case "Image":
                    return ContentType.Image;
                case "Video":
                    return ContentType.Video;
                case "HTML":
                    return ContentType.HTML;
                case "Text":
                    return ContentType.Text;
                case "PPT":
                    return ContentType.PPT;
                default:
                    return ContentType.Nothing;
            }
        }

        /// <summary>
        /// Maps the string action to the content type RippleDictionary.Action
        /// </summary>
        /// <param name="action"></param>
        /// <returns>RippleDictionary.Action</returns>
        /// <exception cref="RippleDictionary.ActionNotKnownException" />
        private static TileAction GetAction(string action)
        {
            switch (action)
            {
                case "Standard":
                    return TileAction.Standard;
                case "Nothing":
                    return TileAction.Nothing;
                case "NothingOnFloor":
                    return TileAction.NothingOnFloor;
                case "HTML":
                    return TileAction.HTML;
                case "QRCode":
                    return TileAction.QRCode;
                case "Logout":
                    return TileAction.Logout;                
                default:
                    throw new TileActionNotKnownException(action);
            }
        }

        /// <summary>
        /// Generates MemoryStream from the xml string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>System.IO.Stream</returns>
        /// <exception cref="RippleDictionary.UnparseableXMLException" />
        private static Stream GenerateRippleDictionaryStreamFromXML(string xml)
        {
            MemoryStream stream = new MemoryStream();

            try
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(xml);
                writer.Flush();
                stream.Position = 0;
            }
            catch (Exception)
            {
                throw new UnparseableXMLException();
            }

            return stream;
        }
        #endregion
    }
}
