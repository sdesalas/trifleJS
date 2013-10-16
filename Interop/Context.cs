using System;
using System.Collections.Generic;
using System.Text;
using Noesis.Javascript;

namespace TrifleJS.Interop
{
    /// <summary>
    /// Defines the V8 JavaScript engine context
    /// </summary>
    public class Context : JavascriptContext
    {
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
