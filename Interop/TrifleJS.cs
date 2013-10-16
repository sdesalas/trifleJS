using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TrifleJS.Interop
{
    /// <summary>
    /// Main library functionality
    /// </summary>
    public class TrifleJS
    {

        public void exit(int exitCode)
        {
            Program.Exit(exitCode);
        }

        public void exit()
        {
            exit(0);
        }

        public void wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public static Dictionary<string, int> version
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
