using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Noesis.Javascript;

namespace TrifleJS.API
{
    /// <summary>
    /// Core JavaScript engine context
    /// </summary>
    public class Context : JavascriptContext
    {
        /// <summary>
        /// Executes a javascript file
        /// </summary>
        /// <param name="filepath">Path to file, this can either be relative to triflejs.exe or current script being executed.</param>
        /// <returns></returns>
        public override object Run(string filepath) {
            FileInfo file = Find(filepath);
            if (file == null || !file.Exists)
            {
                throw new Exception(String.Format("File does not exist {0}.", filepath));
            }
            return Run(File.ReadAllText(file.FullName), file.Name);
        }

        /// <summary>
        /// Finds a file either in the current working directory or in the LibraryPath.
        /// Returns null if not found.
        /// </summary>
        /// <param name="path">path to find</param>
        /// <returns></returns>
        public FileInfo Find(string path) {
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                string currentDir = Environment.CurrentDirectory;
                Environment.CurrentDirectory = Trifle.LibraryPath;
                file = new FileInfo(path);
                Environment.CurrentDirectory = currentDir;
            }
            return file;
        }

        /// <summary>
        /// Gets the current context
        /// </summary>
        public static Context Current {
            get { return Program.context; }
        }

        /// <summary>
        /// Handles an exception
        /// </summary>
        /// <param name="ex"></param>
        public static void Handle(Exception ex)
        {
            JavascriptException jsEx = ex as JavascriptException;
            if (jsEx != null) {
                // Remove refs to Host environment & output javascript error
                string message = jsEx.Message.Replace("TrifleJS.Host+", "");
                Console.error(String.Format("{0} ({1},{2}): {3}", jsEx.Source, jsEx.Line, jsEx.StartColumn, message));
            } else {
                Console.error(String.Format("{0}: {1}", ex.Source, ex.Message));
            }
        }
    }

}
