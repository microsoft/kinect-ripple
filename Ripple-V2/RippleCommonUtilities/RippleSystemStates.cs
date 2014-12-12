using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleCommonUtilities
{
    /// <summary>
    /// States for the Floor
    /// </summary>
    public enum RippleSystemStates
    {
        NoUser,
        UserDetected,
        UserWaitToGoOnStart,
        Start,
        OptionSelected,
        UserPlayingAnimations
    }
}
