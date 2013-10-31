using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TrifleJS.API
{
    /// <summary>
    /// Main library functionality
    /// </summary>
    public class Phantom
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
        /// Executes a JavaScript file in the current context
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool InjectJs(string filename) {
            if (Program.context.Find(filename) != null)
            {
                try
                {
                    Program.context.RunFile(filename);
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
        private static string libraryPath = Environment.CurrentDirectory;

        /// <summary>
        /// Returns the current versions
        /// </summary>
        public static Dictionary<string, int> Version
        {
            get { return new Dictionary<string, int> { 
                    {"major", 1},
                    {"minor", 7},
                    {"patch", 0}
                };
            }
        }

        /// <summary>
        /// Returns the arguments passed when executing triflejs.exe in the console
        /// </summary>
        public static string[] Args {
            get { return Program.args; }
        }

        /// <summary>
        /// Returns the name of the currently executing script
        /// </summary>
        public static string ScriptName {
            get { return scriptName; }
        }
        public static string scriptName;

        /// <summary>
        /// Sets the encoding used for terminal output (default is utf8).
        /// </summary>
        public static string OutputEncoding {
            get { return outputEncoding.WebName; }
            set
            {
                try
                {
                    outputEncoding = Encoding.GetEncoding(value);
                    System.Console.OutputEncoding = outputEncoding;
                }
                catch { };
            }
        }
        public static Encoding outputEncoding = Encoding.UTF8;

        /// <summary>
        /// Controls whether the CookieJar is enabled or not. Defaults to true.
        /// </summary>
        public static bool CookiesEnabled
        {
            get; set;
        }

    }
}
