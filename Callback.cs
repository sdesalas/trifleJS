using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using Noesis.Javascript;
using TrifleJS.API;

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
                        String.Join(",", Context.Parse(arguments))
                    );
                Program.Context.Run(cmd, "Callback#" + id);
            }
            catch (Exception ex) {
                API.Context.Handle(ex);
                return false;
            }
            return true;
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
            /// Handles javascript prompt() functionality.
            /// </summary>
            /// <param name="message"></param>
            public object dialog(string dialog, string message, string defaultValue) {
                try
                {
                    // Execute in V8 engine and return result
                    object result = Program.Context.Run(
                            String.Format("WebPage.onDialog({0}, {1}, {2})", Context.ParseOne(dialog), Context.ParseOne(message), Context.ParseOne(defaultValue)),
                            "WebPage.onDialog()"
                        );
                    return result;
                } catch (Exception ex) {
                    API.Context.Handle(ex);
                }
                return null;
            }

            /// <summary>
            /// Passes control over to page.onCallback() function (if initialized).
            /// </summary>
            /// <param name="jsonArray"></param>
            /// <returns></returns>
            public object callPhantom(string jsonArray) {
                try
                {
                    // Execute in V8 engine and return result
                    object result = Program.Context.Run(
                        String.Format("WebPage.onCallback({0});", jsonArray),
                        "WebPage.onCallback()"
                    );
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
