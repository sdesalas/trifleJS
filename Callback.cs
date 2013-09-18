using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrifleJS
{
    public class Callback
    {
        public static bool execute(string id, params object[] arguments) {

            Program.context.Run(String.Format(@"triflejs.callbacks['{0}'].execute({1});", id, String.Join(",", parse(arguments))));
            return true;
        }

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
    }
}
