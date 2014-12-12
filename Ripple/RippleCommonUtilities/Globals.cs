using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleCommonUtilities
{
    public static class Globals
    {
        public static int PreviouslySelectedBox;
        public static int CurrentlySelectedBox;
        public static RippleSystemStates currentAppState;
        public static DateTime currentBoxTimeStamp;
        public static int CurrentlySelectedParent;
        public static DateTime currentUserTimestamp;
        public static int OptionSelected;
        public static String SelectedOptionFullName;
        public static RippleScreenResoultion CurrentResolution = new RippleScreenResoultion();
        public static String EmpAlias;
        public static String UserName;

        static Globals()
        {
            PreviouslySelectedBox = -1;
            OptionSelected = -1;
            CurrentlySelectedBox = -1;
            currentAppState = RippleSystemStates.NoUser;
            CurrentlySelectedParent = 0;
            currentBoxTimeStamp = DateTime.Now;
            currentUserTimestamp = DateTime.Now;
            SelectedOptionFullName = String.Empty;
            EmpAlias = String.Empty;
            UserName = String.Empty;
        }

        public static void ResetGlobals()
        {
            PreviouslySelectedBox = -1;
            CurrentlySelectedBox = -1;
            currentAppState = RippleSystemStates.NoUser;
            CurrentlySelectedParent = 0;
            currentBoxTimeStamp = DateTime.Now;
            currentUserTimestamp = DateTime.Now;
            EmpAlias = String.Empty;
            SelectedOptionFullName = String.Empty;
            UserName = String.Empty;
        }
    }


}
