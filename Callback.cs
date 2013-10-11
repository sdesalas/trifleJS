using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using Noesis.Javascript;

namespace TrifleJS
{
    public class Callback
    {
        /// <summary>
        /// Allows callback from C# middleware to the V8 JavaScript Runtime.
        /// Deletes the original callback function.
        /// </summary>
        /// <param name="id">Callback id</param>
        /// <param name="arguments">any arguments to pass to the callback</param>
        /// <returns></returns>
        public static bool executeOnce(string id, params object[] arguments)
        {
            return execute(id, true, arguments);
        }

        /// <summary>
        /// Allows callback from C# middleware to the V8 JavaScript Runtime. 
        /// Keeps the original callback function to allow multiple execution.
        /// </summary>
        /// <param name="id">Callback id</param>
        /// <param name="arguments">any arguments to pass to the callback</param>
        /// <returns></returns>
        public static bool execute(string id, params object[] arguments)
        {
            return execute(id, false, arguments);
        }

        /// <summary>
        /// Executes a callback
        /// </summary>
        private static bool execute(string id, bool once, params object[] arguments)
        {
            try
            {
                Program.context.Run(
                    String.Format(@"triflejs.callbacks['{0}'].{1}({2});", 
                        id, 
                        String.Join(",", parse(arguments)), 
                        once ? "executeOnce" :  "execute" 
                    )
                );
            }
            catch (JavascriptException ex) {
                V8.Context.Handle(ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Parses input/output to make it JavaScript friendly
        /// </summary>
        /// <param name="arguments">an array of argument objects (of any type)</param>
        /// <returns>list of parsed arguments</returns>
        public static string[] parse(params object[] arguments) {
            List<string> input = new List<string>();
            foreach (object argument in arguments)
            {
                switch (argument.GetType().Name)
                {
                    case "Int32":
                    case "Double":
                    case "Boolean":
                        input.Add(argument.ToString());
                        break;
                    default:
                        input.Add(String.Format("\"{0}\"", argument.ToString()));
                        break;
                }
            }
            return input.ToArray();
        }

        /// <summary>
        /// Allows callbacks from IE to the V8 Javascript Runtime
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        [ComVisible(true)]
        public class External
        {
            public void xdebug(string message) {
                V8.Console.xdebug(message);
            }
            public void doCallback(string id)
            {
                Callback.execute(id);
            }
        }
    }
}
