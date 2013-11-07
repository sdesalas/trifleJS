using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
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
                input.Add(ParseOne(argument));
            }
            return input.ToArray();
        }

        /// <summary>
        /// Parses input/output tomake it JavaScript friendly
        /// </summary>
        /// <param name="argument">an argument object (of any type)</param>
        /// <returns>the parsed argument</returns>
        public static string ParseOne(object argument) {
            if (argument == null)
            {
                return "null";
            }
            else
            {
                switch (argument.GetType().Name)
                {
                    case "Int32":
                    case "Double":
                        return argument.ToString();
                        break;
                    case "Boolean":
                        return argument.ToString().ToLowerInvariant();
                        break;
                    case "String":
                        // Fix for undefined (coming up as null)
                        if ("{{undefined}}".Equals(argument))
                        {
                            return "undefined";
                        }
                        else
                        {
                            return String.Format("\"{0}\"", argument.ToString());
                        }
                        break;
                    default:
                        return Utils.Serialize(argument);
                        break;
                }
            }
        }

        /// <summary>
        /// Allows callbacks from IE to C# and the V8 Javascript Runtime,
        /// PLEASE NOTE: Debugging inside this class is likely to 
        /// make IE loose focus, causing unexpected behaviour.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        [ComVisible(true)]
        public class External
        {
            /// <summary>
            /// Initialises the ObjectForScripting class
            /// </summary>
            /// <param name="page">A page where scripts are being executed</param>
            public External(API.Modules.WebPage page) {
                this.page = page;
            }
            public API.Modules.WebPage page;
            /// <summary>
            /// Outputs a debug message
            /// </summary>
            /// <param name="message"></param>
            public void xdebug(string message) {
                API.Console.xdebug(message);
            }
            /// <summary>
            /// Performs a callback in V8 environment
            /// </summary>
            /// <param name="id"></param>
            public void doCallback(string id)
            {
                Callback.Execute(id);
            }
            /// <summary>
            /// Passes control over to page.onCallback() function (if initialized).
            /// </summary>
            /// <param name="jsonArray"></param>
            /// <returns></returns>
            public object callPhantom(string jsonArray) {
                try
                {
                    // First, execute the code in V8 engine and obtain result
                    object result = Program.context.Run(
                        String.Format("WebPage.onCallback({0});", jsonArray),
                        "WebPage.onCallback()"
                    );
                    // Then, set the callPhantom.result value for output
                    return result;
                }
                catch (Exception ex) {
                    API.Context.Handle(ex);
                }
                return null;
            }
        }
    }
}
