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
        /// Phantom Cookie Jar
        /// </summary>
        internal static CookieJar cookieJar = new CookieJar();

        /// <summary>
        /// Exists the program
        /// </summary>
        /// <param name="exitCode">Return code</param>
        public void exit(int exitCode)
        {
            Program.Exit(exitCode);
        }

        /// <summary>
        /// Exits the program
        /// </summary>
        public void exit()
        {
            exit(0);
        }

        /// <summary>
        /// Executes a JavaScript file in the current context
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool injectJs(string filename) {
            if (Program.Context.Find(filename) != null)
            {
                try
                {
                    Program.Context.RunFile(filename);
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
        /// Returns the current library path 
        /// (usually the location where a script is being run)
        /// </summary>
        public static string libraryPath { get; set; }

        /// <summary>
        /// Returns the current versions
        /// </summary>
        public static Dictionary<string, int> version
        {
            get { 
                return new Dictionary<string, int> { 
                    {"major", 1},
                    {"minor", 7},
                    {"patch", 0}
                };
            }
        }

        /// <summary>
        /// Loads a module
        /// </summary>
        /// <param name="js"></param>
        public static void LoadModule(string name, string src) {
            try 
            {
                // Fix the path separator
                name = name.Replace("\\", "\\\\");
                string code = "(function(require, exports, module) {" +
                        src +
                        "}.call({}," + 
                        "require," +
                        "require.cache['" + name + "'].exports," +
                        "require.cache['" + name + "']" +
                        "));";
                Program.Context.Run(
                        code
                   );
            } 
            catch (Exception ex) 
            {
                Context.Handle(ex);   
            }
        }

        /// <summary>
        /// Creates a webpage object
        /// </summary>
        /// <returns></returns>
        public static Modules.WebPage createWebPage() {
            return new Modules.WebPage();
        }

        /// <summary>
        /// Creates a webserver object
        /// </summary>
        /// <returns></returns>
        public static Modules.WebServer createWebServer() { 
            return new Modules.WebServer();
        }

        /// <summary>
        /// Returns the arguments passed when executing triflejs.exe in the console
        /// </summary>
        public static string[] args {
            get { return Program.Args; }
        }

        /// <summary>
        /// Returns the name of the currently executing script
        /// </summary>
        public static string scriptName { get; set; }

        /// <summary>
        /// Sets the encoding used for terminal output (default is utf8).
        /// </summary>
        public static string outputEncoding {
            get { return System.Console.OutputEncoding.WebName.ToUpper(); }
            set
            {
                try
                {
                    System.Console.OutputEncoding = Encoding.GetEncoding(value);
                }
                catch {
                    Console.error(String.Format("Unknown Encoding '{0}'", value));
                };
            }
        }

        /// <summary>
        /// Controls whether the CookieJar is enabled or not. Defaults to true.
        /// </summary>
        public static bool cookiesEnabled {
            get { return cookieJar.Enabled; }
            set { cookieJar.Enabled = value; }
        }

    }
}
