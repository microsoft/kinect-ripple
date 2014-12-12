using System;
using System.Collections.Generic;

namespace RippleDictionary
{
    public class Screen
    {
        #region Constructors
        public Screen(Dictionary<string, ScreenContent> screenContents)
        {
            ScreenContents = screenContents;
        } 
        #endregion

        #region Objects
        public Dictionary<string, ScreenContent> ScreenContents; 
        #endregion

        /// <summary>
        /// Method to create or modify the screen content for a given tile
        /// </summary>
        /// <param name="tileID"></param>
        /// <returns></returns>
        public bool CreateOrUpdateScreenContent(String tileID, ScreenContent content)
        {
            if (ScreenContents.ContainsKey(tileID))
            {
                //Update
                ScreenContents[tileID].Id = tileID;
                ScreenContents[tileID].Content = content.Content;
                ScreenContents[tileID].Header = content.Header;
                ScreenContents[tileID].LoopVideo = content.LoopVideo;
                ScreenContents[tileID].Type = content.Type;
            }
            else
            {
                //Create
                ScreenContents.Add(tileID, content);
            }
            return true;
        }
    }

    public class ScreenContent
    {
        #region Fields
        private string id; 
        #endregion

        #region Constructors
        public ScreenContent(ContentType type, string id, string header, string content, bool loopVideo)
        {
            Type = type;
            Id = id;
            Header = header;
            Content = content;
            LoopVideo = loopVideo;
        } 
        #endregion

        #region Properties
        public ContentType Type
        {
            get;
            set;
        }

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

        public string Header
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public bool LoopVideo
        {
            get;
            set;
        }
        #endregion
    }
}
