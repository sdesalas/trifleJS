using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace TrifleJS
{
    /// <summary>
    /// Defines how the JavaScript context interacts with C# objects.
    /// </summary>
    public class Host
    {
        /// <summary>
        /// Defines functionality for the javascript 'console' object.
        /// </summary>
        public class console {
            public static void debug(object value)
            {
                stdout(value, ConsoleColor.DarkCyan);
            }
            public static void xdebug(object value)
            {
                Utils.Debug(String.Format("{0}", parse(value)));
            }
            public static void log(object value)
            {
                stdout(value, null);
            }
            public static void error(object value) {
                stderr(value, ConsoleColor.Red);
            }
            public static void warn(object value)
            {
                stdout(value, ConsoleColor.Yellow);
            }
            public static void clear()
            {
                Console.Clear();
            }
            public static void wait(int milliseconds) {
                System.Threading.Thread.Sleep(milliseconds);
            }
            private static void stdout(object value, ConsoleColor? color) {
                std(value, color, false);
            }
            private static void stderr(object value, ConsoleColor? color)
            {
                std(value, color, true);
            }
            private static void std(object value, ConsoleColor? color, bool err) {
                ConsoleColor normalColor = Console.ForegroundColor;
                if (color != null) { Console.ForegroundColor = (ConsoleColor)color; }
                if (err) { Console.Error.WriteLine("{0}", parse(value)); }
                else { Console.WriteLine("{0}", parse(value)); }
                Console.ForegroundColor = normalColor;
            }
            private static object parse(object value) {
                if (value == null)
                {
                    return "null";
                }
                string type = value.GetType().ToString();
                switch (type)
                {
                    case "System.String":
                    case "System.Boolean":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Double":
                        return value;
                        break;
                    default:
                        return new JavaScriptSerializer().Serialize(value);
                        break;
                }
            }
        }

        /// <summary>
        /// Defines core library functionality
        /// </summary>
        public class triflejs { 
        
            public void exit() {
                Environment.Exit(0);
            }

        }

        /// <summary>
        /// Defines a set of external classes that can be instantiated in the javascript engine.
        /// </summary>
        public class interop
        {
            public WebPage WebPage()
            {
                return new WebPage();
            }
            public FileSystem FileSystem()
            {
                return new FileSystem();
            }
        }

        public static void Handle(Noesis.Javascript.JavascriptException ex)
        {
            // Remove refs to Host environment & output javascript error
            string message = ex.Message.Replace("TrifleJS.Host+", "");
            Host.console.error(String.Format("{0} ({1},{2}): {3}", ex.Source, ex.Line, ex.StartColumn, message));
        }

    }

}
