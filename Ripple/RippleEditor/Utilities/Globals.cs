using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleEditor.Utilities
{
    public enum RippleSystemStates
    {
        None,
        ActionContent,
        Start,
        OptionSelected,
    }

    public static class Globals
    {
        public static bool IsProjectOpen = false;
        public static String CurrentFileLocation = String.Empty;
        public static TemplateOptions CurrentTemplate = TemplateOptions.Template_3X3;
        public static int PreviouslySelectedBox;
        public static int CurrentlySelectedBox;
        public static RippleSystemStates currentAppState;
        public static int CurrentlySelectedParent;
        public static int OptionSelected;

        public static void ResetGlobals()
        {
            IsProjectOpen = false;
            CurrentTemplate = TemplateOptions.Template_3X3;
            CurrentFileLocation = String.Empty;
            PreviouslySelectedBox = -1;
            OptionSelected = -1;
            CurrentlySelectedBox = -1;
            currentAppState = RippleSystemStates.Start;
            CurrentlySelectedParent = 0;
        }
    }

    
}
