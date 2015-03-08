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
        /// A queue of pending callbacks that should be executed in
        /// the V8 environment. This is used for event handling on 
        /// modules that use a separate thread (ie. ChildProcess), 
        /// as making the callback inside those threads does not
        /// expose our current V8 environment.
        /// </summary>
        public static Queue<Callback> queue = new Queue<Callback>();

        /// <summary>
        /// Adds items to the callback queue
        /// </summary>
        /// <param name="id"></param>
        /// <param name="arguments"></param>
        public static void Queue(string id, bool once, params object[] arguments) 
        {
            queue.Enqueue(new Callback { Id = id, Once = once, Arguments = arguments } );
        }

        /// <summary>
        /// Processes the callback queue 
        /// (used in the event loop)
        /// </summary>
        public static void ProcessQueue()
        {
            while (queue.Count > 0)
            {
                Callback callback = queue.Dequeue();
                Callback.Execute(callback.Id, callback.Once, callback.Arguments);
            }
        }

        /// <summary>
        /// Used for queueing callbacks
        /// </summary>
        internal string Id {get; set;}
        internal bool Once { get; set; }
        internal object[] Arguments { get; set; }

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
            /// Fires an event in the page object (V8 runtime) passing some arguments
            /// </summary>
            /// <param name="shortname"></param>
            /// <param name="jsonArgs"></param>
            /// <returns></returns>
            public object fireEvent(string nickname, string jsonArgs) {
                try
                {
                    // Execute in V8 engine and return result
                    object result = Program.Context.Run(
                        String.Format("WebPage.fireEvent('{0}', '{1}', {2});", nickname, page.uuid, jsonArgs),
                        "WebPage.fireEvent('" + nickname + "')"
                    );
                    return result;
                }
                catch (Exception ex)
                {
                    API.Context.Handle(ex);
                }
                return null;
            }

        }
    }
}
