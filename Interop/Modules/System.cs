using System;
using System.Collections.Generic;
using System.Text;

namespace TrifleJS.Interop.Modules
{
    /// <summary>
    /// Defines a set of system tools 
    /// </summary>
    public class System
    {

        public static string[] args
        {
            get { return Program.args; }
        }

    }
}
