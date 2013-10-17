using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace TrifleJS.API
{
    /// <summary>
    /// Main library functionality
    /// </summary>
    public class Trifle
    {
        /// <summary>
        /// Exists the program
        /// </summary>
        /// <param name="exitCode">Return code</param>
        public void Exit(int exitCode)
        {
            Program.Exit(exitCode);
        }

        /// <summary>
        /// Exits the program
        /// </summary>
        public void Exit()
        {
            Exit(0);
        }

        /// <summary>
        /// Suspends current thread
        /// </summary>
        /// <param name="milliseconds">Milliseconds to wait for</param>
        public void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Executes a JavaScript file in the current context
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool InjectJs(string filename) {
            if (Program.context.Find(filename) != null)
            {
                try
                {
                    Program.context.Run(filename);
                    return true;
                }
                catch (Exception ex)
                {
                    Context.Handle(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the current library path (usually the location where a script is being run)
        /// </summary>
        public static string LibraryPath
        {
            get { return libraryPath; }
            set { libraryPath = value; }
        }
        private static string libraryPath;

        /// <summary>
        /// Returns the current versions
        /// </summary>
        public static Dictionary<string, int> Version
        {
            get { return new Dictionary<string, int> { 
                    {"major", 0},
                    {"minor", 2},
                    {"patch", 0}
                };
            }
        }
    }
}
