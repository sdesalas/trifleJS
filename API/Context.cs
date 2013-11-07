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
        public object RunFile(string filepath)
        {
            FileInfo file = Find(filepath);
            if (file == null || !file.Exists)
            {
                throw new Exception(String.Format("File does not exist {0}.", filepath));
            }
            // Read file 
            string script = File.ReadAllText(file.FullName);
            // Execute file
            return RunScript(script, file.Name);
        }

        /// <summary>
        /// Executes a script
        /// </summary>
        /// <param name="script">String containing javascript to execute</param>
        /// <param name="scriptName">Name of the script</param>
        public object RunScript(string script, string scriptName)
        {
            // Read file and add a blank function at the end, 
            // this is to fix a stackoverflow bug
            // in Javascript.NET where it tries
            // to return an object with circular reference
            script += ";(function() {})();";
            // Tag which file we are executing
            Phantom.scriptName = scriptName;
            // Execute 
            return base.Run(script, scriptName);
        }

        /// <summary>
        /// Finds a file either in the current working directory or in the LibraryPath.
        /// Returns null if not found.
        /// </summary>
        /// <param name="path">path to find</param>
        /// <returns></returns>
        public FileInfo Find(string path)
        {
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                string currentDir = Environment.CurrentDirectory;
                Environment.CurrentDirectory = Phantom.LibraryPath;
                file = new FileInfo(path);
                Environment.CurrentDirectory = currentDir;
            }
            return file;
        }

        /// <summary>
        /// Gets the current context
        /// </summary>
        public static Context Current
        {
            get { return Program.context; }
        }

        /// <summary>
        /// Handles an exception
        /// </summary>
        /// <param name="ex"></param>
        public static void Handle(Exception ex)
        {
            JavascriptException jsEx = ex as JavascriptException;
            if (jsEx != null)
            {
                // Remove refs to Host environment & output javascript error
                string message = jsEx.Message.Replace("TrifleJS.Host+", "");
                Console.error(String.Format("{0} ({1},{2}): {3}", jsEx.Source, jsEx.Line, jsEx.StartColumn, message));
            }
            else
            {
                Console.error(String.Format("{0}: {1}", ex.Source, ex.Message));
            }
        }

    }

}
