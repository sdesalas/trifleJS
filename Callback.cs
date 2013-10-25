using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
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
        public static bool ExecuteOnce(string id, params object[] arguments)
        {
            return Execute(id, true, arguments);
        }

        /// <summary>
        /// Allows callback from C# middleware to the V8 JavaScript Runtime. 
        /// Keeps the original callback function to allow multiple execution.
        /// </summary>
        /// <param name="id">Callback id</param>
        /// <param name="arguments">any arguments to pass to the callback</param>
        /// <returns></returns>
        public static bool Execute(string id, params object[] arguments)
        {
            return Execute(id, false, arguments);
        }

        /// <summary>
        /// Executes a callback
        /// </summary>
        public static bool Execute(string id, bool once, params object[] arguments)
        {
            try
            {
                if (arguments == null) { arguments = new object[0]; }
                String cmd = String.Format(@"trifle.Callback.{0}('{1}', [{2}]);",
                        once ? "executeOnce" : "execute",
                        id,
                        String.Join(",", Parse(arguments))
                    );
                Program.context.Run(cmd, "Callback#" + id);
            }
            catch (Exception ex) {
                API.Context.Handle(ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Parses input/output to make it JavaScript friendly
        /// </summary>
        /// <param name="arguments">an array of argument objects (of any type)</param>
        /// <returns>list of parsed arguments</returns>
        public static string[] Parse(params object[] arguments) {
            List<string> input = new List<string>();
            foreach (object argument in arguments)
            {
                if (argument == null) {
                    input.Add("null");
                }
                else
                {
                    switch (argument.GetType().Name)
                    {
                        case "Int32":
                        case "Double":
                            input.Add(argument.ToString());
                            break;
                        case "Boolean":
                            input.Add(argument.ToString().ToLowerInvariant());
                            break;
                        case "String":
                            // Fix for undefined (coming up as null)
                            if ("{{undefined}}".Equals(argument))
                            {
                                input.Add("undefined");
                            }
                            else {
                                input.Add(String.Format("\"{0}\"", argument.ToString()));
                            }
                            break;
                        default:
                            input.Add(JsonConvert.SerializeObject(argument));
                            break;
                    }
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
                API.Console.xdebug(message);
            }
            public void doCallback(string id)
            {
                Callback.Execute(id);
            }
        }
    }
}
