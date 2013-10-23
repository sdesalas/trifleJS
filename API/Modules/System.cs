using System;
using System.Collections.Generic;
using System.Text;

namespace TrifleJS.API.Modules
{
    /// <summary>
    /// Defines a set of system tools 
    /// </summary>
    public class System
    {
        /// <summary>
        /// Returns the arguments passed when executing triflejs.exe in the console
        /// </summary>
        public static string[] Args
        {
            get { return Program.args; }
        }

    }
}
