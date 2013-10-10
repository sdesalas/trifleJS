using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace TrifleJS
{
    public class Callback
    {
        /// <summary>
        /// Allows callback from C# middleware to the V8 JavaScript Runtime
        /// </summary>
        /// <param name="id">Callback id</param>
        /// <param name="arguments">any arguments to pass to the callback</param>
        /// <returns></returns>
        public static bool execute(string id, params object[] arguments) {
            try
            {
                Program.context.Run(String.Format(@"triflejs.callbacks['{0}'].execute({1});", id, String.Join(",", parse(arguments))));
            }
            catch (Noesis.Javascript.JavascriptException ex) {
                Host.Handle(ex);
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
                Host.console.xdebug(message);
            }
            public void doCallback(string id)
            {
                Callback.execute(id);
            }
        }
    }
}
