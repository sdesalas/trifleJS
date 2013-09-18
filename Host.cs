using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace TrifleJS
{
    public class Host
    {
        public class console {
            public static void debug(object value)
            {
                Console.WriteLine("{0}", parse(value));
            }
            public static void log(object value)
            {
                Console.WriteLine("{0}", parse(value));
            }
            public static void error(object value) {
                ConsoleColor normalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("{0}", parse(value));
                Console.ForegroundColor = normalColor;
            }
            public static void warning(object value)
            {
                ConsoleColor normalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}", parse(value));
                Console.ForegroundColor = normalColor;
            }
            public static void clear()
            {
                Console.Clear();
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

        public class phantom { 
        
            public void exit() {
                Environment.Exit(0);
            }
        }

        public class require
        {
            public object create(string module)
            {
                if (module == null)
                {
                    throw new Exception("Please provide a module reference for require(module)");
                }
                else { }
                return (new Module(module)).create();
            }
        }

        public class _interop
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

    }

}
