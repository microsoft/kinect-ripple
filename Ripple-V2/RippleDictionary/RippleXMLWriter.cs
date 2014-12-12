using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RippleDictionary
{
    public static class RippleXMLWriter
    {
        /// <summary>
        /// Writes the ripple object onto an XML file as out.xml and returns true if successful.
        /// </summary>
        /// <param name="ripple">RippleDictionary.Ripple</param>
        /// <returns>System.bool</returns>
        public static bool TryWriteToXML(Ripple ripple, string filePath)
        {
            XDocument document = new XDocument();

            try
            {
                XElement eFloor = GetXElementFloor(ripple.Floor);
                XElement eScreen = GetXElementScreen(ripple.Screen);

                document.Add(new XComment(string.Format("Ripple XML generated on {0}", DateTime.Now)));
                document.Add(new XElement(XMLElementsAndAttributes.Ripple, eFloor, eScreen));

                document.Save(filePath);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("RippleDictionary", "Could not write to the RippleXML file.\n" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
        }

        #region Helpers
        /// <summary>
        /// Initializes an instance of Floor in XElement format with specified contents and attributes.
        /// </summary>
        /// <param name="floor">RippleDictionary.Floor</param>
        /// <returns>System.Xml.Linq.XElement</returns>
        private static XElement GetXElementFloor(Floor floor)
        {
            XElement eStart = GetXElementStart(floor.Start);

            XElement eTransition = new XElement(XMLElementsAndAttributes.Transition,
                new XAttribute(XMLElementsAndAttributes.Music, floor.Transition.Music),
                new XAttribute(XMLElementsAndAttributes.Animation, floor.Transition.Animation)
                );

            XElement eLockingPeriod = new XElement(XMLElementsAndAttributes.LockingPeriod, floor.LockingPeriod.ToString());

            XElement eSystemAutoLockPeriod = new XElement(XMLElementsAndAttributes.SystemAutoLockPeriod, floor.SystemAutoLockPeriod.ToString());

            XElement eUpperTile = new XElement(XMLElementsAndAttributes.UpperTile,
                new XAttribute(XMLElementsAndAttributes.Id, floor.UpperTile.Id),
                new XAttribute(XMLElementsAndAttributes.Name, floor.UpperTile.Name),
                new XAttribute(XMLElementsAndAttributes.TileType, floor.UpperTile.TileType),
                new XAttribute(XMLElementsAndAttributes.Content, floor.UpperTile.Content),
                new XAttribute(XMLElementsAndAttributes.CorrespondingScreenContentType, floor.UpperTile.CorrespondingScreenContentType),
                new XAttribute(XMLElementsAndAttributes.Color, floor.UpperTile.Color),               
                new XAttribute(XMLElementsAndAttributes.Style, GenerateStringValue(floor.UpperTile.Style)),
                new XAttribute(XMLElementsAndAttributes.Coordinate, GenerateStringValue(floor.UpperTile.Coordinate)),
                new XAttribute(XMLElementsAndAttributes.Action, floor.UpperTile.Action),
                new XAttribute(XMLElementsAndAttributes.ActionURI, floor.UpperTile.ActionURI)
                );

            XElement eTiles = new XElement(XMLElementsAndAttributes.Tiles, GetXElementTiles(floor.Tiles));

            return new XElement(XMLElementsAndAttributes.Floor, new XElement(XMLElementsAndAttributes.SetupID, floor.SetupID), eStart, eTransition, eLockingPeriod, eSystemAutoLockPeriod, eUpperTile, eTiles);
        }

        /// <summary>
        /// Initializes an instance of Start in XElement format with specified contents and attributes.
        /// </summary>
        /// <param name="start">RippleDictionary.Start</param>
        /// <returns></returns>
        private static XElement GetXElementStart(Start start)
        {
            XElement eAnimation = new XElement(XMLElementsAndAttributes.Animation,
                new XAttribute(XMLElementsAndAttributes.Name, start.Animation.Name),
                new XAttribute(XMLElementsAndAttributes.Content, start.Animation.Content),
                new XAttribute(XMLElementsAndAttributes.AnimationType, start.Animation.AnimType.ToString())
                );

            XElement eUnlock = new XElement(XMLElementsAndAttributes.Unlock,
                new XAttribute(XMLElementsAndAttributes.Mode, start.Unlock.Mode),
                new XAttribute(XMLElementsAndAttributes.UnlockType, start.Unlock.UnlockType)
                );

            XElement eIntroVideoWaitPeriod = new XElement(XMLElementsAndAttributes.IntroVideoWaitPeriod,
                new XAttribute(XMLElementsAndAttributes.Value, start.IntroVideoWaitPeriod.ToString())
                );

            return new XElement(XMLElementsAndAttributes.Start, eAnimation, eUnlock, eIntroVideoWaitPeriod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        private static List<XElement> GetXElementTiles(Dictionary<string, Tile> tiles)
        {
            List<XElement> eTiles = new List<XElement>();
            XElement eTile = null;

            foreach (var tile in tiles)
            {
                eTile = GetXElementTile(tile.Value);

                eTiles.Add(eTile);
            }

            return eTiles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        private static XElement GetXElementTile(Tile tile)
        {
            XAttribute eId, eName, eTileType, eColor, eStyle, eCoordinate, eAction, eActionURI, eContent, eCorrespondingScreenContentType;
            List<XElement> eSubTiles = new List<XElement>();
            string id = null, appendSub = null;

            id = tile.Id;
            appendSub = null;
            eId = new XAttribute(XMLElementsAndAttributes.Id, tile.Id);
            eName = new XAttribute(XMLElementsAndAttributes.Name, tile.Name);
            eTileType = new XAttribute(XMLElementsAndAttributes.TileType, tile.TileType.ToString());
            if (tile.Content != null)
                eContent = new XAttribute(XMLElementsAndAttributes.Content, tile.Content);
            else
                eContent = null;
            if (tile.CorrespondingScreenContentType != null)
                eCorrespondingScreenContentType = new XAttribute(XMLElementsAndAttributes.CorrespondingScreenContentType, tile.CorrespondingScreenContentType.ToString());
            else
                eCorrespondingScreenContentType = null;
            eColor = new XAttribute(XMLElementsAndAttributes.Color, tile.Color.ToString());
            eStyle = new XAttribute(XMLElementsAndAttributes.Style, GenerateStringValue(tile.Style));
            eCoordinate = new XAttribute(XMLElementsAndAttributes.Coordinate, GenerateStringValue(tile.Coordinate));
            eAction = new XAttribute(XMLElementsAndAttributes.Action, tile.Action.ToString());
            eActionURI = new XAttribute(XMLElementsAndAttributes.ActionURI, tile.ActionURI);         

            if (tile.SubTiles != null && tile.SubTiles.Count != 0)
            {
                eSubTiles = GetXElementTiles(tile.SubTiles);
            }
            else
            {
                eSubTiles = null;
            }

            for (int i = 0; i < ((id.Length - id.Replace(XMLElementsAndAttributes.Sub, "").Length) / XMLElementsAndAttributes.Sub.Length); i++)
            {
                appendSub += XMLElementsAndAttributes.Sub;
            }

            return new XElement(appendSub + XMLElementsAndAttributes.Tile, eSubTiles, eId, eName, eTileType, eContent, eCorrespondingScreenContentType, eColor, eStyle, eCoordinate, eAction, eActionURI);
        }

        /// <summary>
        /// Initializes an instance of Screen in XElement format with specified contents and attributes.
        /// </summary>
        /// <param name="screen">System.Xml.Linq.Screen</param>
        /// <returns>RippleDictionary.Screen</returns>
        private static XElement GetXElementScreen(Screen screen)
        {
            List<XElement> eScreenContents = new List<XElement>();
            XElement eScreenContent = null;
            XAttribute eType, eId, eHeader, eContent, eLoopVideo;

            foreach (var screenContent in screen.ScreenContents)
            {
                eType = new XAttribute(XMLElementsAndAttributes.Type, screenContent.Value.Type.ToString());
                eId = new XAttribute(XMLElementsAndAttributes.Id, screenContent.Value.Id);
                eHeader = new XAttribute(XMLElementsAndAttributes.Header, screenContent.Value.Header);
                eContent = new XAttribute(XMLElementsAndAttributes.Content, screenContent.Value.Content);
                if (screenContent.Value.LoopVideo != null)
                {
                    eLoopVideo = new XAttribute(XMLElementsAndAttributes.LoopVideo, screenContent.Value.LoopVideo);
                }
                else
                {
                    eLoopVideo = null;
                }

                eScreenContent = new XElement(XMLElementsAndAttributes.ScreenContent, eType, eId, eHeader, eContent, eLoopVideo);

                eScreenContents.Add(eScreenContent);
            }
            return new XElement(XMLElementsAndAttributes.Screen, eScreenContents);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GenerateStringValue(object obj)
        {
            string value = "";
            foreach (var property in obj.GetType().GetProperties())
            {
                value += property.Name + ":'" + property.GetValue(obj, null).ToString() + "'; ";
            }

            return value.Trim();
        } 
        #endregion
    }
}
