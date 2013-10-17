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

        public void Exit(int exitCode)
        {
            Program.Exit(exitCode);
        }

        public void Exit()
        {
            Exit(0);
        }

        public void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public bool InjectJs(string filename) {
            if (File.Exists(filename))
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


        private static string libraryPath;

        public static string LibraryPath
        {
            get { return libraryPath; }
            set { libraryPath = value; }
        }

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
