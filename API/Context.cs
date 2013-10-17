using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Noesis.Javascript;

namespace TrifleJS.API
{
    /// <summary>
    /// Defines the V8 JavaScript engine context
    /// </summary>
    public class Context : JavascriptContext
    {
        public object Run(string filePath) {
            FileInfo file = new FileInfo(filePath);
            if (filePath == null || !file.Exists)
            {
                throw new Exception(String.Format("File does not exist {0}.", filePath));
            }
            return Run(File.ReadAllText(filePath), file.Name);
        }

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
