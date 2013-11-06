using System;
using System.Collections.Generic;
using System.Text;

namespace TrifleJS.API
{
    /// <summary>
    /// Defines functionality for the javascript 'console' object.
    /// </summary>
    public class Console
    {
        public static void debug(object value)
        {
            StdOut(value, ConsoleColor.DarkCyan);
        }

        public static void xdebug(object value)
        {
            Utils.Debug(String.Format("{0}", Parse(value)));
        }

        public static void log(object value)
        {
            StdOut(value, null);
        }

        public static void error(object value)
        {
            StdErr(value, ConsoleColor.Red);
        }

        public static void warn(object value)
        {
            StdOut(value, ConsoleColor.Yellow);
        }

        public static void color(string color, object value)
        {
            ConsoleColor consoleColor = ConsoleColor.Gray;
            try
            {
                consoleColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color, true);
            }
            catch { }
            finally {
                StdOut(value, consoleColor);
            }
        }

        public static void clear()
        {
            System.Console.Clear();
        }

        private static void StdOut(object value, ConsoleColor? color)
        {
            Std(value, color, false);
        }

        private static void StdErr(object value, ConsoleColor? color)
        {
            Std(value, color, true);
        }

        private static void Std(object value, ConsoleColor? color, bool err)
        {
            ConsoleColor normalColor = System.Console.ForegroundColor;
            if (color != null) { System.Console.ForegroundColor = (ConsoleColor)color; }
            if (err) { System.Console.Error.WriteLine("{0}", Parse(value)); }
            else { System.Console.WriteLine("{0}", Parse(value)); }
            System.Console.ForegroundColor = normalColor;
        }

        /// <summary>
        /// Parses an object for output to the system console
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object Parse(object value)
        {
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
                default:
                    return Utils.Serialize(value);
            }
        }
    }
}
