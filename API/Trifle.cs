using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace TrifleJS.API
{
    /// <summary>
    /// Trifle Class
    /// </summary>
    public class Trifle
    {
        /// <summary>
        /// Returns the current versions
        /// </summary>
        public static Dictionary<string, int> Version
        {
            get
            {
                return new Dictionary<string, int> { 
                    {"major", 0},
                    {"minor", 5},
                    {"patch", 0}
                };
            }
        }

        /// <summary>
        /// Version of Internet Explorer currently being emulated.
        /// </summary>
        public static string Emulation { get; set; }

        /// <summary>
        /// Suspends V8 execution and runs program event loop
        /// </summary>
        /// <param name="milliseconds">Milliseconds to wait for</param>
        public static void Wait(int milliseconds)
        {
            int now = Environment.TickCount;
            while (Environment.TickCount < now + milliseconds) {
                Program.DoEvents();
            }
        }

        /// <summary>
        /// Used to ignore SSL errors
        /// </summary>
        public static bool IgnoreSSLErrors
        {
            set
            {
                Native.Browser.IgnoreSSLErrors = value;
            }
        }

        // These are a set of C# mid-tier classes that can be instantiated
        // inside the javascript engine as CommonJS Modules.

        public Modules.WebPage WebPage()
        {
            return new Modules.WebPage();
        }

        public Modules.FileSystem FileSystem()
        {
            if (!Program.SecureMode)
            {
                return new Modules.FileSystem();
            }
            throw new Exception("--secure-mode: FileSystem module has been disabled.");
        }

        public Modules.System System()
        {
            return new Modules.System();
        }

        public Modules.WebServer WebServer()
        {
            return new Modules.WebServer();
        }

        public Modules.ChildProcess ChildProcess()
        {
            if (!Program.SecureMode)
            {
                return new Modules.ChildProcess();
            }
            throw new Exception("--secure-mode: ChildProcess module has been disabled.");
        }
    }
}
