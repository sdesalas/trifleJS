using System;
using System.Collections.Generic;
using System.Text;
using Noesis.Javascript;

namespace TrifleJS.V8
{
    /// <summary>
    /// Defines the V8 JavaScript engine context
    /// </summary>
    public class Context : JavascriptContext
    {
        public static void Handle(JavascriptException ex)
        {
            // Remove refs to Host environment & output javascript error
            string message = ex.Message.Replace("TrifleJS.Host+", "");
            Console.error(String.Format("{0} ({1},{2}): {3}", ex.Source, ex.Line, ex.StartColumn, message));
        }
    }

}
