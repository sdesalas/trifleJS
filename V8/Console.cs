using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Text;

namespace TrifleJS.V8
{
    /// <summary>
    /// Defines functionality for the javascript 'console' object.
    /// </summary>
    public class Console
    {
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

        public static void error(object value)
        {
            stderr(value, ConsoleColor.Red);
        }

        public static void warn(object value)
        {
            stdout(value, ConsoleColor.Yellow);
        }

        public static void clear()
        {
            System.Console.Clear();
        }

        private static void stdout(object value, ConsoleColor? color)
        {
            std(value, color, false);
        }

        private static void stderr(object value, ConsoleColor? color)
        {
            std(value, color, true);
        }

        private static void std(object value, ConsoleColor? color, bool err)
        {
            ConsoleColor normalColor = System.Console.ForegroundColor;
            if (color != null) { System.Console.ForegroundColor = (ConsoleColor)color; }
            if (err) { System.Console.Error.WriteLine("{0}", parse(value)); }
            else { System.Console.WriteLine("{0}", parse(value)); }
            System.Console.ForegroundColor = normalColor;
        }

        private static object parse(object value)
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
                    return new JavaScriptSerializer().Serialize(value);
            }
        }
    }
}
